import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { z } from "zod";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { servicioSubastas } from "@/servicios/servicioSubastas";
import { servicioCategorias } from "@/servicios/servicioCategorias";
import { useUsuario } from "@/contextos/contextoUsuario";
import { formatoMoneda } from "@/utilidades/formatoMonedaYFecha";
import type { Categoria } from "@/tipos/subastaTipos";

export const Route = createFileRoute("/publicar")({
  head: () => ({
    meta: [
      { title: "Publicar una subasta | SubastaYa" },
      {
        name: "description",
        content:
          "Cargá tu producto, definí precio base, incremento mínimo y la ventana temporal de la subasta.",
      },
      { property: "og:title", content: "Publicar una subasta | SubastaYa" },
      {
        property: "og:description",
        content: "Formulario de publicación con validaciones en pantalla para vendedores.",
      },
    ],
  }),
  component: Publicar,
});

const conversionAFechaEntradaLocal = (fecha: Date) =>
  new Date(fecha.getTime() - fecha.getTimezoneOffset() * 60000)
    .toISOString()
    .slice(0, 16);

const esquemaValidacion = z
  .object({
    titulo: z
      .string()
      .trim()
      .min(4, "El título debe tener al menos 4 caracteres")
      .max(90, "El título no puede superar los 90 caracteres"),
    descripcion: z
      .string()
      .trim()
      .min(20, "Contá algo más del producto (mín. 20 caracteres)")
      .max(1200),
    imagenUrl: z.string().trim().refine(
      (val) => val.startsWith("data:image/webp;base64,") || val.startsWith("http"),
      "Debe subir una imagen o ingresar una URL válida"
    ),
    categoria: z.string().min(1, "Elegí una categoría"),
    precioInicial: z.number().positive("El precio base debe ser mayor a 0"),
    incrementoMinimo: z.number().positive("El incremento mínimo debe ser mayor a 0"),
    fechaInicio: z.string().min(1, "Fecha de inicio requerida"),
    fechaFin: z.string().min(1, "Fecha de fin requerida"),
  })
  .refine(
    (v) => new Date(v.fechaFin).getTime() > new Date(v.fechaInicio).getTime(),
    {
      path: ["fechaFin"],
      message: "La finalización debe ser posterior al inicio",
    },
  )
  .refine(
    (v) =>
      new Date(v.fechaFin).getTime() - new Date(v.fechaInicio).getTime() >=
      60_000,
    {
      path: ["fechaFin"],
      message: "La subasta debe durar al menos 1 minuto",
    },
  )
  .refine((v) => v.incrementoMinimo <= v.precioInicial, {
    path: ["incrementoMinimo"],
    message: "El incremento no puede superar al precio base",
  });

type ClaveCampo =
  | "titulo"
  | "descripcion"
  | "imagenUrl"
  | "categoria"
  | "precioInicial"
  | "incrementoMinimo"
  | "fechaInicio"
  | "fechaFin";

type ErroresFormulario = { [K in ClaveCampo]?: string | undefined };

/**
 * Procesa la imagen seleccionada por el usuario: la redimensiona y convierte a formato WebP.
 * Asegura que el tamaño final del string codificado en base64 no exceda los 64KB.
 */
const procesarImagenWebp = (archivo: File): Promise<string> => {
  return new Promise((resolve, reject) => {
    const lector = new FileReader();
    lector.onload = (evento) => {
      const img = new Image();
      img.onload = () => {
        let ancho = img.width;
        let alto = img.height;
        // Reducimos el tamaño inicial a un máximo razonable para evitar cuelgues
        const tamañoMaximo = 800;
        if (ancho > tamañoMaximo || alto > tamañoMaximo) {
          const proporcion = Math.min(tamañoMaximo / ancho, tamañoMaximo / alto);
          ancho = Math.round(ancho * proporcion);
          alto = Math.round(alto * proporcion);
        }

        const canvas = document.createElement("canvas");
        canvas.width = ancho;
        canvas.height = alto;
        const ctx = canvas.getContext("2d");
        if (!ctx) return reject(new Error("Error al procesar la imagen"));

        let calidad = 0.8;
        let iteracion = 0;
        const iterarCompresion = () => {
          ctx.clearRect(0, 0, ancho, alto);
          ctx.drawImage(img, 0, 0, ancho, alto);
          const dataUrl = canvas.toDataURL("image/webp", calidad);
          
          // Se verifica el tamaño de la cadena en base64 para que no exceda 64KB (65536 caracteres)
          if (dataUrl.length <= 65536 || iteracion >= 10 || calidad <= 0.1) {
            if (dataUrl.length > 65536) {
              return reject(new Error("La imagen es muy compleja y excede el límite. Intentá con otra."));
            }
            resolve(dataUrl);
          } else {
            // Si el tamaño supera el límite, se reduce la resolución y la calidad iterativamente
            ancho = Math.round(ancho * 0.8);
            alto = Math.round(alto * 0.8);
            canvas.width = ancho;
            canvas.height = alto;
            calidad -= 0.1;
            iteracion++;
            iterarCompresion();
          }
        };
        iterarCompresion();
      };
      img.onerror = () => reject(new Error("El archivo seleccionado no es una imagen válida"));
      img.src = evento.target?.result as string;
    };
    lector.onerror = () => reject(new Error("Hubo un error al leer el archivo"));
    lector.readAsDataURL(archivo);
  });
};

function Publicar() {
  const navigate = useNavigate();
  const { usuarioActual } = useUsuario();
  const fechaActual = new Date();

  const [listaCategorias, setListaCategorias] = useState<Categoria[]>([]);
  const [publicando, setPublicando] = useState(false);

  const [formulario, setFormulario] = useState({
    titulo: "",
    descripcion: "",
    imagenUrl: "",
    categoria: "",
    precioInicial: "",
    incrementoMinimo: "",
    fechaInicio: conversionAFechaEntradaLocal(new Date(fechaActual.getTime() + 60_000)),
    fechaFin: conversionAFechaEntradaLocal(new Date(fechaActual.getTime() + 60 * 60_000)),
  });

  const [errores, setErrores] = useState<ErroresFormulario>({});

  useEffect(() => {
    servicioCategorias
      .listado()
      .then((categorias) => {
        setListaCategorias(categorias);
        if (categorias.length > 0) {
          setFormulario((f) => ({
            ...f,
            categoria: f.categoria || String(categorias[0]?.id ?? 1),
          }));
        }
      })
      .catch(() => {});
  }, []);

  const eventoActualizacionCampo =
    (campo: keyof typeof formulario) => (valor: string) => {
      setFormulario((f) => ({ ...f, [campo]: valor }));
      setErrores((e) => ({ ...e, [campo]: undefined }));
    };

  const eventoArchivo = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const archivo = e.target.files?.[0];
    if (!archivo) return;
    
    try {
      const dataUrl = await procesarImagenWebp(archivo);
      eventoActualizacionCampo("imagenUrl")(dataUrl);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Error al procesar la imagen");
      e.target.value = ""; // Limpia el input si falla
    }
  };

  const eventoEnvioFormulario = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!usuarioActual) {
      toast.error("Seleccioná un usuario activo en el encabezado.");
      return;
    }

    const resultadoParseo = esquemaValidacion.safeParse({
      titulo: formulario.titulo,
      descripcion: formulario.descripcion,
      imagenUrl: formulario.imagenUrl,
      categoria: formulario.categoria,
      precioInicial: Number(formulario.precioInicial),
      incrementoMinimo: Number(formulario.incrementoMinimo),
      fechaInicio: formulario.fechaInicio,
      fechaFin: formulario.fechaFin,
    });

    if (!resultadoParseo.success) {
      const proximosErrores: ErroresFormulario = {};
      for (const issue of resultadoParseo.error.issues) {
        proximosErrores[issue.path[0] as ClaveCampo] = issue.message;
      }
      setErrores(proximosErrores);
      toast.error("Revisá el formulario", { description: "Hay campos con errores." });
      return;
    }

    setPublicando(true);
    try {
      const categoriaId = Number(resultadoParseo.data.categoria) || 1;
      const nuevaSubasta = await servicioSubastas.creacion({
        titulo: resultadoParseo.data.titulo,
        descripcion: resultadoParseo.data.descripcion,
        precioInicial: resultadoParseo.data.precioInicial,
        incrementoMinimo: resultadoParseo.data.incrementoMinimo,
        imagenUrl: resultadoParseo.data.imagenUrl,
        categoriaId: categoriaId,
        fechaInicio: new Date(resultadoParseo.data.fechaInicio).toISOString(),
        fechaFin: new Date(resultadoParseo.data.fechaFin).toISOString(),
        vendedorId: usuarioActual.id,
      });

      toast.success("¡Subasta publicada!", {
        description: `"${nuevaSubasta.titulo}" ya está disponible.`,
      });

      navigate({ to: "/subasta/$id", params: { id: String(nuevaSubasta.id) } });
    } catch (error: unknown) {
      const mensaje = error instanceof Error ? error.message : "Error al publicar la subasta.";
      toast.error("Error al publicar", { description: mensaje });
    } finally {
      setPublicando(false);
    }
  };

  const vistaPreviaPrecio =
    Number(formulario.precioInicial) > 0 ? Number(formulario.precioInicial) : 0;

  return (
    <main className="mx-auto max-w-5xl px-4 py-10">
      <h1 className="text-3xl font-bold md:text-4xl">Publicar una subasta</h1>
      <p className="mt-2 text-muted-foreground">
        Definí el producto, la configuración económica y la ventana temporal.
      </p>

      <form onSubmit={eventoEnvioFormulario} className="mt-8 grid gap-6 lg:grid-cols-[1.4fr_1fr]" noValidate>
        <div className="surface-card space-y-5 rounded-xl p-5">
          <CampoFormulario etiqueta="Título del producto" error={errores.titulo}>
            <Input
              value={formulario.titulo}
              maxLength={90}
              onChange={(e) => eventoActualizacionCampo("titulo")(e.target.value)}
              placeholder="Ej. iPhone 15 Pro 256GB titanio"
            />
          </CampoFormulario>

          <CampoFormulario etiqueta="Descripción detallada" error={errores.descripcion}>
            <Textarea
              value={formulario.descripcion}
              maxLength={1200}
              rows={5}
              onChange={(e) => eventoActualizacionCampo("descripcion")(e.target.value)}
              placeholder="Estado, accesorios, garantía, detalles relevantes…"
            />
          </CampoFormulario>

          <div className="grid gap-5 sm:grid-cols-2">
            <CampoFormulario etiqueta="Imagen del producto" error={errores.imagenUrl}>
              <Input
                type="file"
                accept="image/*"
                onChange={eventoArchivo}
                className="cursor-pointer"
              />
            </CampoFormulario>
            <CampoFormulario etiqueta="Categoría" error={errores.categoria}>
              <Select
                value={formulario.categoria}
                onValueChange={eventoActualizacionCampo("categoria")}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Elegí una categoría" />
                </SelectTrigger>
                <SelectContent>
                  {listaCategorias.map((c) => (
                    <SelectItem key={c.id} value={String(c.id)}>
                      {c.nombre}
                    </SelectItem>
                  ))}
                  {listaCategorias.length === 0 && (
                    <SelectItem value="1">General</SelectItem>
                  )}
                </SelectContent>
              </Select>
            </CampoFormulario>
          </div>

          <div className="grid gap-5 sm:grid-cols-2">
            <CampoFormulario etiqueta="Precio base inicial" error={errores.precioInicial}>
              <Input
                type="number"
                min={1}
                value={formulario.precioInicial}
                onChange={(e) => eventoActualizacionCampo("precioInicial")(e.target.value)}
                placeholder="100000"
              />
            </CampoFormulario>
            <CampoFormulario etiqueta="Incremento mínimo por puja" error={errores.incrementoMinimo}>
              <Input
                type="number"
                min={1}
                value={formulario.incrementoMinimo}
                onChange={(e) => eventoActualizacionCampo("incrementoMinimo")(e.target.value)}
                placeholder="5000"
              />
            </CampoFormulario>
          </div>

          <div className="grid gap-5 sm:grid-cols-2">
            <CampoFormulario etiqueta="Inicio" error={errores.fechaInicio}>
              <Input
                type="datetime-local"
                value={formulario.fechaInicio}
                onChange={(e) => eventoActualizacionCampo("fechaInicio")(e.target.value)}
              />
            </CampoFormulario>
            <CampoFormulario etiqueta="Finalización" error={errores.fechaFin}>
              <Input
                type="datetime-local"
                value={formulario.fechaFin}
                onChange={(e) => eventoActualizacionCampo("fechaFin")(e.target.value)}
              />
            </CampoFormulario>
          </div>

          <Button type="submit" size="lg" className="w-full" disabled={publicando}>
            {publicando ? "Publicando en backend…" : "Publicar subasta"}
          </Button>
        </div>

        <aside className="surface-card h-fit overflow-hidden rounded-xl">
          <div className="aspect-[4/3] bg-muted">
            {formulario.imagenUrl ? (
              <img
                src={formulario.imagenUrl}
                alt="Vista previa del producto"
                className="size-full object-cover"
              />
            ) : (
              <div className="flex size-full items-center justify-center text-sm text-muted-foreground">
                Vista previa de la imagen
              </div>
            )}
          </div>
          <div className="space-y-2 p-4">
            <p className="text-xs uppercase tracking-wider text-muted-foreground">Vista previa</p>
            <p className="font-semibold">{formulario.titulo || "Título del producto"}</p>
            <p className="font-display text-xl font-bold text-primary">
              {formatoMoneda(vistaPreviaPrecio)}
            </p>
            <p className="text-xs text-muted-foreground">
              Cada puja sube al menos {formatoMoneda(Number(formulario.incrementoMinimo) || 0)}. Si alguien oferta en
              los últimos 60 segundos, el reloj se extiende automáticamente.
            </p>
          </div>
        </aside>
      </form>
    </main>
  );
}

function CampoFormulario({
  etiqueta,
  error,
  children,
}: {
  etiqueta: string;
  error?: string | undefined;
  children: React.ReactNode;
}) {
  return (
    <div className="space-y-2">
      <Label>{etiqueta}</Label>
      {children}
      {error && <p className="text-xs font-medium text-destructive">{error}</p>}
    </div>
  );
}
