using GSS.Core.Compiler.Diagnostics;

namespace GSS.Sharp.Diagnostics
{
	public static class SemanticDiagnosticRules
	{
		public static readonly DiagnosticDescriptor TypeMismatch = new(
			DiagnosticCode.Sem300_TypeMismatch, DiagnosticSeverity.Error,
			"Type mismatch: cannot convert from '{0}' to '{1}'.", "Ensure types match or provide an explicit conversion.");

		public static readonly DiagnosticDescriptor UndefinedVariable = new(
			DiagnosticCode.Sem301_UndefinedVariable, DiagnosticSeverity.Error,
			"The name '{0}' does not exist in the current context.", "Declare the variable before using it.");

		public static readonly DiagnosticDescriptor VariableAlreadyDeclared = new(
			DiagnosticCode.Sem302_VariableAlreadyDeclared, DiagnosticSeverity.Error,
			"A local variable named '{0}' is already defined in this scope.", "Rename the variable.");

		public static readonly DiagnosticDescriptor CannotAssignReadOnly = new(
			DiagnosticCode.Sem303_CannotAssignReadOnly, DiagnosticSeverity.Error,
			"Property or variable '{0}' is read-only.", "Ensure the target has a setter or use a different variable.");

		public static readonly DiagnosticDescriptor UndefinedMethod = new(
			DiagnosticCode.Sem304_UndefinedMethod, DiagnosticSeverity.Error,
			"Method '{0}' does not exist in the API profile '{1}'.", "Check the spelling or verify the API bindings.");

		public static readonly DiagnosticDescriptor ArgumentCountMismatch = new(
			DiagnosticCode.Sem305_ArgumentCountMismatch, DiagnosticSeverity.Error,
			"No overload for method '{0}' takes {1} arguments.", "Pass the exact number of arguments required.");

		public static readonly DiagnosticDescriptor InvalidOperator = new(
			DiagnosticCode.Sem306_InvalidOperator, DiagnosticSeverity.Error,
			"Operator '{0}' cannot be applied to operand(s) of type '{1}'.", "Check operator compatibility.");

		public static readonly DiagnosticDescriptor ImplicitVarWithoutInitializer = new(
			DiagnosticCode.Sem307_ImplicitVarWithoutInitializer, DiagnosticSeverity.Error,
			"Implicitly-typed variables must be initialized.", "Assign a value during declaration.");

		public static readonly DiagnosticDescriptor InvalidAssignmentTarget = new(
			DiagnosticCode.Sem308_InvalidAssignmentTarget, DiagnosticSeverity.Error,
			"The left-hand side of an assignment must be a variable, property, or indexer. Invalid target for {0}.", "Verify that you are assigning to a valid location.");

		public static readonly DiagnosticDescriptor UninitializedVariable = new(
			(DiagnosticCode)309, DiagnosticSeverity.Error,
			"Use of unassigned local variable '{0}'.", "Ensure the variable is initialized before accessing it.");

		public static readonly DiagnosticDescriptor InvalidBreakContext = new(
			DiagnosticCode.Sem310_InvalidBreakContext, DiagnosticSeverity.Error,
			"No enclosing loop out of which to break.", "Ensure 'break' is used inside a loop (while/for).");

		public static readonly DiagnosticDescriptor InvalidContinueContext = new(
			DiagnosticCode.Sem311_InvalidContinueContext, DiagnosticSeverity.Error,
			"No enclosing loop out of which to continue.", "Ensure 'continue' is used inside a loop (while/for).");

		public static readonly DiagnosticDescriptor FeatureNotSupported = new(
			(DiagnosticCode)312, DiagnosticSeverity.Error,
			"Feature '{0}' is not yet supported by the GSS compiler.", "Avoid using this feature for now.");
	}
}