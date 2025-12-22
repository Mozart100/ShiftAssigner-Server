using System;

namespace ShiftAssignerServer.Services.Validation;
public class ShiftAssignmentException : Exception
{
     public ShiftAssignmentException(params ShiftAssignmentError[] shiftAssignmentErrors)
     {
         ShiftAssignmentErrors = shiftAssignmentErrors;
     }

     public ShiftAssignmentError[] ShiftAssignmentErrors { get; }

 }
