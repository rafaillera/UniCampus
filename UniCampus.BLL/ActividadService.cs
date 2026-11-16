using System;
using System.Collections.Generic;
using System.Linq;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reglas de negocio de las actividades académicas.
    /// Esta capa NO contiene SQL ni referencias a System.Web: valida los datos
    /// y delega la persistencia al repositorio de la capa DAL.
    /// </summary>
    public class ActividadService
    {
        public const int LongitudMaximaTitulo = 150;
        public const int LongitudMaximaDescripcion = 1000;
        public const int LongitudMaximaLugar = 150;
        public const int CupoMaximoPermitido = 1000;

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

        /// <summary>
        /// Lista las actividades publicadas.
        /// </summary>
        public List<ActividadDto> ListarActividades()
        {
            return _repository.ObtenerPublicadas();
        }

        /// <summary>
        /// Devuelve una actividad por su Id, o null si el Id no es válido o no existe.
        /// </summary>
        public ActividadDto ObtenerActividad(int actividadId)
        {
            return actividadId > 0 ? _repository.ObtenerPorId(actividadId) : null;
        }

        /// <summary>
        /// Valida la actividad en el servidor y, solo si es válida, la inserta.
        /// Si hay errores de validación se devuelven sin llamar a la capa DAL.
        /// </summary>
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

        /// <summary>
        /// Reglas de validación del servidor. Se expone como público para
        /// facilitar pruebas unitarias sin base de datos.
        /// </summary>
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
                errores.Add("La fecha de inicio no puede ser anterior a la fecha actual.");

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
