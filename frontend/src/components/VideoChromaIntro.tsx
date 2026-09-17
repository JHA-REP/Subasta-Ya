import React, { useEffect, useRef, useState } from "react";
import { X, Volume2, VolumeX } from "lucide-react";

interface VideoChromaIntroProps {
  videoSrc?: string;
  onFinished?: () => void;
}

export function VideoChromaIntro({
  videoSrc = "/video.mp4",
  onFinished,
}: VideoChromaIntroProps) {
  const videoRef = useRef<HTMLVideoElement | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const animFrameIdRef = useRef<number | null>(null);
  const rvfcIdRef = useRef<number | null>(null);

  const [visible, setVisible] = useState(true);
  const [fading, setFading] = useState(false);
  const [hasStarted, setHasStarted] = useState(false);
  const [isMuted, setIsMuted] = useState(false);
  const [isReady, setIsReady] = useState(false);

  // Cerrar y desvanecer el video
  const handleClose = (e?: React.MouseEvent) => {
    e?.stopPropagation();
    setFading(true);
    setTimeout(() => {
      setVisible(false);
      if (videoRef.current) {
        videoRef.current.pause();
      }
      onFinished?.();
    }, 500);
  };

  const toggleMute = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (!videoRef.current) return;
    const nextMuted = !videoRef.current.muted;
    videoRef.current.muted = nextMuted;
    setIsMuted(nextMuted);
  };

  // Iniciar video y audio juntos desde el segundo 0 al hacer clic
  const startVideoWithAudio = async () => {
    const video = videoRef.current;
    if (!video || hasStarted) return;

    try {
      video.currentTime = 0;
      video.muted = false;
      setIsMuted(false);
      await video.play();
      setHasStarted(true);
    } catch {
      try {
        video.muted = true;
        setIsMuted(true);
        await video.play();
        setHasStarted(true);
      } catch (err) {
        console.warn("No se pudo iniciar la reproducción:", err);
      }
    }
  };

  useEffect(() => {
    const video = videoRef.current;
    const canvas = canvasRef.current;
    if (!video || !canvas) return;

    let isDestroyed = false;

    // Intentar inicializar WebGL para aceleración por GPU a 60 FPS
    let gl: WebGLRenderingContext | null = null;
    try {
      gl = canvas.getContext("webgl", {
        alpha: true,
        premultipliedAlpha: false,
        antialias: false,
        powerPreference: "high-performance",
      });
    } catch {
      gl = null;
    }

    let renderFrame: () => void = () => {};

    if (gl) {
      // === RENDERIZADOR ACELERADO POR GPU (WebGL Shader) ===
      const vertexShaderSource = `
        attribute vec2 a_position;
        attribute vec2 a_texCoord;
        varying vec2 v_texCoord;
        void main() {
          gl_Position = vec4(a_position, 0.0, 1.0);
          // Invertir Y para mapear correctamente coordenadas de video a WebGL
          v_texCoord = vec2(a_texCoord.x, 1.0 - a_texCoord.y);
        }
      `;

      // Fragment shader GLSL: Chroma key en tiempo real sin costo de CPU
      const fragmentShaderSource = `
        precision mediump float;
        uniform sampler2D u_image;
        varying vec2 v_texCoord;

        void main() {
          vec4 color = texture2D(u_image, v_texCoord);
          float r = color.r;
          float g = color.g;
          float b = color.b;
          float total = r + g + b;

          if (total > 0.001) {
            float maxRB = max(r, b);
            float greenDominance = g - maxRB;
            float normG = g / total;

            // Detección de fondo verde con desvanecimiento de bordes
            if (normG > 0.37 && greenDominance > 0.024 && g > 0.15) {
              if (normG >= 0.42 || greenDominance >= 0.078) {
                gl_FragColor = vec4(0.0, 0.0, 0.0, 0.0);
                return;
              } else {
                float factor = clamp((0.42 - normG) / 0.05, 0.0, 1.0);
                color.a = factor;
                color.g = maxRB; // Des-spill: remover reflejos verdes en el borde
              }
            } else if (greenDominance > 0.0 && g > 0.24) {
              color.g = mix(maxRB, g, 0.3);
            }
          }

          gl_FragColor = color;
        }
      `;

      const createShader = (type: number, source: string) => {
        const shader = gl!.createShader(type);
        if (!shader) return null;
        gl!.shaderSource(shader, source);
        gl!.compileShader(shader);
        return shader;
      };

      const vertShader = createShader(gl.VERTEX_SHADER, vertexShaderSource);
      const fragShader = createShader(gl.FRAGMENT_SHADER, fragmentShaderSource);

      if (vertShader && fragShader) {
        const program = gl.createProgram();
        if (program) {
          gl.attachShader(program, vertShader);
          gl.attachShader(program, fragShader);
          gl.linkProgram(program);
          gl.useProgram(program);

          // Buffer de geometría (cuadrilátero)
          const positionBuffer = gl.createBuffer();
          gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
          gl.bufferData(
            gl.ARRAY_BUFFER,
            new Float32Array([
              -1, -1, 0, 0,
               1, -1, 1, 0,
              -1,  1, 0, 1,
              -1,  1, 0, 1,
               1, -1, 1, 0,
               1,  1, 1, 1,
            ]),
            gl.STATIC_DRAW
          );

          const aPosition = gl.getAttribLocation(program, "a_position");
          const aTexCoord = gl.getAttribLocation(program, "a_texCoord");

          gl.enableVertexAttribArray(aPosition);
          gl.vertexAttribPointer(aPosition, 2, gl.FLOAT, false, 16, 0);

          gl.enableVertexAttribArray(aTexCoord);
          gl.vertexAttribPointer(aTexCoord, 2, gl.FLOAT, false, 16, 8);

          // Textura de video
          const texture = gl.createTexture();
          gl.bindTexture(gl.TEXTURE_2D, texture);
          gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
          gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
          gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
          gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);

          renderFrame = () => {
            if (isDestroyed || !video || !canvas || !gl) return;
            if (video.readyState < HTMLMediaElement.HAVE_CURRENT_DATA) return;

            const width = video.videoWidth;
            const height = video.videoHeight;
            if (!width || !height) return;

            if (canvas.width !== width || canvas.height !== height) {
              canvas.width = width;
              canvas.height = height;
              gl.viewport(0, 0, width, height);
            }

            // Subir frame de video directamente a la GPU
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, video);

            // Dibujar con shader GPU
            gl.drawArrays(gl.TRIANGLES, 0, 6);
          };
        }
      }
    }

    // Fallback Canvas 2D en caso de que WebGL no estuviese disponible
    if (!gl) {
      const ctx = canvas.getContext("2d", { willReadFrequently: true });
      renderFrame = () => {
        if (isDestroyed || !video || !canvas || !ctx) return;
        if (video.readyState < HTMLMediaElement.HAVE_CURRENT_DATA) return;

        const width = video.videoWidth;
        const height = video.videoHeight;
        if (!width || !height) return;

        if (canvas.width !== width || canvas.height !== height) {
          canvas.width = width;
          canvas.height = height;
        }

        ctx.drawImage(video, 0, 0, width, height);
        const frame = ctx.getImageData(0, 0, width, height);
        const data = frame.data;
        const len = data.length;

        for (let i = 0; i < len; i += 4) {
          const r = data[i];
          const g = data[i + 1];
          const b = data[i + 2];
          const total = r + g + b;
          if (total === 0) continue;

          const maxRB = r > b ? r : b;
          const greenDominance = g - maxRB;
          const normG = g / total;

          if (normG > 0.37 && greenDominance > 6 && g > 40) {
            if (normG >= 0.42 || greenDominance >= 20) {
              data[i + 3] = 0;
            } else {
              const factor = (0.42 - normG) / 0.05;
              data[i + 3] = Math.round(data[i + 3] * Math.max(0, Math.min(1, factor)));
              data[i + 1] = maxRB;
            }
          }
        }
        ctx.putImageData(frame, 0, 0);
      };
    }

    // Bucle sincronizado a la tasa de refresco nativa del video
    const animationLoop = () => {
      if (isDestroyed) return;
      renderFrame();

      if ("requestVideoFrameCallback" in video) {
        // @ts-expect-error - requestVideoFrameCallback nativo
        rvfcIdRef.current = video.requestVideoFrameCallback(animationLoop);
      } else {
        animFrameIdRef.current = requestAnimationFrame(animationLoop);
      }
    };

    const handleLoadedData = () => {
      setIsReady(true);
      renderFrame();
    };

    const handleEnded = () => {
      handleClose();
    };

    video.addEventListener("loadeddata", handleLoadedData);
    video.addEventListener("ended", handleEnded);

    if (video.readyState >= HTMLMediaElement.HAVE_CURRENT_DATA) {
      setIsReady(true);
      renderFrame();
    }

    if ("requestVideoFrameCallback" in video) {
      // @ts-expect-error - requestVideoFrameCallback
      rvfcIdRef.current = video.requestVideoFrameCallback(animationLoop);
    } else {
      animFrameIdRef.current = requestAnimationFrame(animationLoop);
    }

    return () => {
      isDestroyed = true;
      video.removeEventListener("loadeddata", handleLoadedData);
      video.removeEventListener("ended", handleEnded);
      if (animFrameIdRef.current) cancelAnimationFrame(animFrameIdRef.current);
      if (rvfcIdRef.current && "cancelVideoFrameCallback" in video) {
        // @ts-expect-error - cancelVideoFrameCallback
        video.cancelVideoFrameCallback(rvfcIdRef.current);
      }
    };
  }, []);

  // Listener para iniciar con clic en cualquier parte de la pantalla
  useEffect(() => {
    if (hasStarted) return;

    const handleWindowClick = (e: MouseEvent | TouchEvent) => {
      const target = e.target as HTMLElement | null;
      if (target?.closest("[data-skip-button='true']")) {
        return;
      }
      startVideoWithAudio();
    };

    window.addEventListener("click", handleWindowClick, { capture: true });
    window.addEventListener("touchstart", handleWindowClick, { capture: true });

    return () => {
      window.removeEventListener("click", handleWindowClick, { capture: true });
      window.removeEventListener("touchstart", handleWindowClick, { capture: true });
    };
  }, [hasStarted]);

  if (!visible) return null;

  return (
    <div
      className={`fixed inset-0 z-[9999] flex items-center justify-center transition-opacity duration-700 select-none ${
        fading ? "opacity-0 pointer-events-none" : "opacity-100"
      } ${!hasStarted ? "cursor-pointer" : "pointer-events-none"}`}
      aria-label="Video de bienvenida"
    >
      {/* Video fuente oculto que alimenta al canvas */}
      <video
        ref={videoRef}
        src={videoSrc}
        playsInline
        preload="auto"
        className="hidden"
      />

      {/* Controles flotantes superiores */}
      <div className="absolute top-6 right-6 flex items-center gap-2 pointer-events-auto z-20">
        {hasStarted && (
          <button
            type="button"
            onClick={toggleMute}
            className="flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-black/60 hover:bg-black/80 text-white backdrop-blur-md text-xs font-medium border border-white/20 shadow-lg transition-all hover:scale-105 active:scale-95"
            title={isMuted ? "Activar sonido" : "Silenciar"}
          >
            {isMuted ? (
              <>
                <VolumeX className="w-4 h-4 text-amber-300" />
                <span>Activar sonido</span>
              </>
            ) : (
              <>
                <Volume2 className="w-4 h-4 text-emerald-400" />
                <span>Silenciar</span>
              </>
            )}
          </button>
        )}

        <button
          type="button"
          data-skip-button="true"
          onClick={handleClose}
          className="flex items-center gap-1 px-3.5 py-1.5 rounded-full bg-black/60 hover:bg-black/80 text-white backdrop-blur-md text-xs font-medium border border-white/20 shadow-lg transition-all hover:scale-105 active:scale-95"
          title="Omitir video"
        >
          <span>Omitir</span>
          <X className="w-4 h-4 text-zinc-300" />
        </button>
      </div>

      {/* Canvas centrado con renderizado WebGL por GPU ultra fluido */}
      <div className="relative flex items-center justify-center max-w-[92vw] max-h-[88vh] pointer-events-none">
        <canvas
          ref={canvasRef}
          className={`max-w-full max-h-[85vh] w-auto h-auto object-contain filter drop-shadow-[0_15px_30px_rgba(0,0,0,0.35)] transition-opacity duration-300 ${
            isReady ? "opacity-100" : "opacity-0"
          }`}
        />
      </div>
    </div>
  );
}
