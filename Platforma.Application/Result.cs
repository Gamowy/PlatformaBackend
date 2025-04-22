using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }
        public static Result<T> Success(T Value)
        {
            return new Result<T> { IsSuccess = true, Value = Value };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T> { IsSuccess = false, Error = error };
        }
    }
}
