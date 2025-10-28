namespace EstancieroResponses
{
    public class ApiResponse<T>
    {   //Sirve como plantilla para las respuestas de la API
        //Indica si la operación fue exitosa, un mensaje opcional, los datos devueltos y una lista de errores si los hay
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        //Lista de errores para manejar múltiples errores en una sola respuesta
        public ApiResponse()
        {
            Errors = new List<string>();
        }
        //Constructor para respuestas exitosas con datos
        public ApiResponse(T data, string? message = null)
        {
            Success = true;
            Data = data;
            Message = message;
            Errors = new List<string>();
        }
        //Constructor para respuestas fallidas con mensaje y lista de errores
        public ApiResponse(string message, List<string>? errors = null)
        {
            Success = false;
            Message = message;
            Errors = errors ?? new List<string>();
        }
    }
}
