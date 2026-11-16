using System;
using System.Collections.Generic;
using System.Linq;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    public class ActividadService
    {
        public const int LongitudMaximaTitulo = 150;
        public const int LongitudMaximaDescripcion = 1000;
        public const int LongitudMaximaLugar = 150;
        public const int CupoMaximoPermitido = 1000;

        public const string MensajeFechaInicioPasada = "La fecha de inicio no puede ser anterior a la fecha actual.";

        private readonly ActividadRepository _repository;
        private readonly TipoActividadRepository _tipoRepository;

        public ActividadService()
            : this(new ActividadRepository(), new TipoActividadRepository())
        {
        }

        public ActividadService(ActividadRepository repository, TipoActividadRepository tipoRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _tipoRepository = tipoRepository ?? throw new ArgumentNullException(nameof(tipoRepository));
        }

        public List<ActividadDto> ListarActividades()
        {
            return _repository.ObtenerPublicadas();
        }

        public ActividadDto ObtenerActividad(int actividadId)
        {
            return actividadId > 0 ? _repository.ObtenerPorId(actividadId) : null;
        }

        public ResultadoOperacion RegistrarActividad(ActividadDto dto)
        {
            if (dto == null)
            {
                return ResultadoOperacion.Fallo("No se recibieron los datos de la actividad.");
            }

            Normalizar(dto);

            List<string> errores = Validar(dto);
            if (errores.Count > 0)
            {
                return ResultadoOperacion.Fallo(string.Join(" ", errores));
            }

            if (!_tipoRepository.Existe(dto.TipoId))
            {
                return ResultadoOperacion.Fallo("El tipo de actividad seleccionado no existe o está inactivo.");
            }

            int nuevoId = _repository.Insertar(dto);
            return ResultadoOperacion.Ok("La actividad \"" + dto.Titulo + "\" se registró correctamente.", nuevoId);
        }

        public List<string> Validar(ActividadDto dto)
        {
            var errores = new List<string>();

            // Título
            if (string.IsNullOrWhiteSpace(dto.Titulo))
                errores.Add("El título es obligatorio.");
            else if (dto.Titulo.Length > LongitudMaximaTitulo)
                errores.Add("El título no puede superar " + LongitudMaximaTitulo + " caracteres.");

            // Descripción (opcional)
            if (dto.Descripcion != null && dto.Descripcion.Length > LongitudMaximaDescripcion)
                errores.Add("La descripción no puede superar " + LongitudMaximaDescripcion + " caracteres.");

            // Tipo
            if (dto.TipoId <= 0)
                errores.Add("Debe seleccionar el tipo de actividad.");

            // Fechas
            bool tieneFechaInicio = dto.FechaInicio != DateTime.MinValue;
            bool tieneFechaFin = dto.FechaFin != DateTime.MinValue;

            if (!tieneFechaInicio)
                errores.Add("La fecha de inicio es obligatoria.");
            else if (dto.FechaInicio.Date < DateTime.Today)
                errores.Add(MensajeFechaInicioPasada);

            if (!tieneFechaFin)
                errores.Add("La fecha de finalización es obligatoria.");
            else if (tieneFechaInicio && dto.FechaFin.Date < dto.FechaInicio.Date)
                errores.Add("La fecha de finalización no puede ser anterior a la fecha de inicio.");

            // Horas (opcionales, pero coherentes si la actividad dura un solo día)
            if (dto.HoraInicio.HasValue && dto.HoraFin.HasValue
                && tieneFechaInicio && tieneFechaFin
                && dto.FechaInicio.Date == dto.FechaFin.Date
                && dto.HoraFin.Value <= dto.HoraInicio.Value)
            {
                errores.Add("La hora de finalización debe ser posterior a la hora de inicio.");
            }

            // Modalidad
            if (string.IsNullOrWhiteSpace(dto.Modalidad))
                errores.Add("La modalidad es obligatoria.");
            else if (!Modalidades.Todas.Contains(dto.Modalidad))
                errores.Add("La modalidad seleccionada no es válida.");

            // Lugar
            if (string.IsNullOrWhiteSpace(dto.Lugar))
                errores.Add("El lugar (o enlace de la sesión virtual) es obligatorio.");
            else if (dto.Lugar.Length > LongitudMaximaLugar)
                errores.Add("El lugar no puede superar " + LongitudMaximaLugar + " caracteres.");

            // Cupos
            if (dto.Cupos <= 0)
                errores.Add("Los cupos deben ser un número entero mayor que 0.");
            else if (dto.Cupos > CupoMaximoPermitido)
                errores.Add("Los cupos no pueden superar " + CupoMaximoPermitido + ".");

            // Estado
            if (!EstadosActividad.Todos.Contains(dto.Estado))
                errores.Add("El estado seleccionado no es válido.");

            return errores;
        }

        public List<ActividadDto> ListarTodas()
        {
            return _repository.ObtenerTodas();
        }

        public ResultadoOperacion ActualizarActividad(ActividadDto dto)
        {
            if (dto == null || dto.ActividadId <= 0)
            {
                return ResultadoOperacion.Fallo("No se recibió la actividad que desea actualizar.");
            }

            ActividadDto actual = _repository.ObtenerPorId(dto.ActividadId);
            if (actual == null)
            {
                return ResultadoOperacion.Fallo("La actividad que intenta editar no existe.");
            }

            Normalizar(dto);
            dto.Estado = actual.Estado;

            List<string> errores = Validar(dto);

            if (dto.FechaInicio.Date == actual.FechaInicio.Date)
            {
                errores.Remove(MensajeFechaInicioPasada);
            }

            if (dto.Cupos > 0 && dto.Cupos < actual.Inscritos)
            {
                errores.Add("No puede reducir los cupos a " + dto.Cupos + ": ya hay "
                    + actual.Inscritos + " estudiantes inscritos.");
            }

            if (errores.Count > 0)
            {
                return ResultadoOperacion.Fallo(string.Join(" ", errores));
            }

            if (!_tipoRepository.Existe(dto.TipoId))
            {
                return ResultadoOperacion.Fallo("El tipo de actividad seleccionado no existe o está inactivo.");
            }

            if (!_repository.Actualizar(dto))
            {
                return ResultadoOperacion.Fallo("No se pudo actualizar: los cupos quedarían por debajo de los inscritos. Recargue la página e intente de nuevo.");
            }

            return ResultadoOperacion.Ok("La actividad \"" + dto.Titulo + "\" se actualizó correctamente.", dto.ActividadId);
        }

        public ResultadoOperacion CambiarEstado(int actividadId, string nuevoEstado)
        {
            if (!EstadosActividad.Todos.Contains(nuevoEstado))
            {
                return ResultadoOperacion.Fallo("El estado seleccionado no es válido.");
            }

            ActividadDto actual = ObtenerActividad(actividadId);
            if (actual == null)
            {
                return ResultadoOperacion.Fallo("La actividad no existe.");
            }

            if (actual.Estado == nuevoEstado)
            {
                return ResultadoOperacion.Fallo("La actividad ya está en estado " + nuevoEstado + ".");
            }

            if (nuevoEstado == EstadosActividad.Publicada && actual.FechaFin.Date < DateTime.Today)
            {
                return ResultadoOperacion.Fallo("No se puede publicar una actividad que ya terminó.");
            }

            _repository.CambiarEstado(actividadId, nuevoEstado);

            string aviso = (nuevoEstado == EstadosActividad.Inactiva || nuevoEstado == EstadosActividad.Cancelada) && actual.Inscritos > 0
                ? " Tenía " + actual.Inscritos + " estudiante(s) inscrito(s)."
                : string.Empty;

            return ResultadoOperacion.Ok("La actividad \"" + actual.Titulo + "\" quedó en estado " + nuevoEstado + "." + aviso, actividadId);
        }

        private static void Normalizar(ActividadDto dto)
        {
            dto.Titulo = Recortar(dto.Titulo);
            dto.Descripcion = Recortar(dto.Descripcion);
            dto.Lugar = Recortar(dto.Lugar);
            dto.Modalidad = Recortar(dto.Modalidad);
            dto.Estado = string.IsNullOrWhiteSpace(dto.Estado) ? EstadosActividad.Publicada : dto.Estado.Trim();
        }

        private static string Recortar(string valor)
        {
            return valor == null ? null : valor.Trim();
        }
    }
}