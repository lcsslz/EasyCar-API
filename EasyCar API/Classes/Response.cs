namespace EasyCar_API
{
    public class Response
    {
        public bool contains_response { get;  }
        public List<object>? response_list { get; }
        public string? error_message { get; }

        public Response(string error_message)
        {
            contains_response = false;
            this.error_message = error_message;
        }

        public Response(object response_object)
        {
            contains_response = true;
            response_list = new List<object> { response_object };
        }

        public Response(List<object> response_list)
        {
            contains_response = true;
            this.response_list = response_list;
        }
    }
}