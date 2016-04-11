
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
    
public partial class Transaction
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Transaction()
    {

        this.ReturnLinks = new HashSet<ReturnLink>();

    }


    public long Id { get; set; }

    public long purchaserId { get; set; }

    public Nullable<long> requestedForUserId { get; set; }

    public long productId { get; set; }

    public int quantity { get; set; }

    public decimal unitPrice { get; set; }

    public System.DateTime purchaseDate { get; set; }

    public long cartId { get; set; }

    public bool isDeleted { get; set; }



    public virtual Product Product { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ReturnLink> ReturnLinks { get; set; }

    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

}

}
