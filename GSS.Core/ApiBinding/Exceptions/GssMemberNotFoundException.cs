using GSS.Core.Exceptions;

namespace GSS.Core.ApiBinding.Exceptions
{
	public class GssMemberNotFoundException : GssException
	{
		public GssMemberNotFoundException(string memberName, string context)
			: base($"GSS Binding Error: Member '{memberName}' not found. Context: {context}") { }
	}
}