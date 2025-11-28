using System;

namespace ShiftAssignerServer.Services.Validation;

 public class ShiftAssignmentError
 {
     public ShiftAssignmentError(string propertyName, string errorMessage)
     {
         PropertyName = propertyName;
         ErrorMessage = errorMessage;
     }

     public string ErrorMessage { get; }
     public string PropertyName { get; }

 }
public class ShiftAssignmentException : Exception
{
     public ShiftAssignmentException(params ShiftAssignmentError[] shiftAssignmentErrors)
     {
         ShiftAssignmentErrors = shiftAssignmentErrors;
     }

     public ShiftAssignmentError[] ShiftAssignmentErrors { get; }

 }
