//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Financial_Management_Application.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReturnLink
    {
        public long Id { get; set; }
        public long transactionId { get; set; }
        public long returnStateId { get; set; }
        public System.DateTime dateStateChanged { get; set; }
    
        public virtual ReturnState ReturnState { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
