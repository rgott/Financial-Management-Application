
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
    
public partial class User
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public User()
    {

        this.Transactions = new HashSet<Transaction>();

        this.Transactions1 = new HashSet<Transaction>();

        this.UserClaims = new HashSet<UserClaim>();

        this.UserLogins = new HashSet<UserLogin>();

        this.Roles = new HashSet<Role>();

    }


    public long Id { get; set; }

    public long Address { get; set; }

    public long Division { get; set; }

    public System.DateTime TimeZoneOffset { get; set; }

    public string Login { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public Nullable<System.DateTime> CreationDate { get; set; }

    public Nullable<System.DateTime> ApprovalDate { get; set; }

    public Nullable<System.DateTime> ExpireDate { get; set; }

    public Nullable<System.DateTime> LastLoginDate { get; set; }

    public bool IsLocked { get; set; }

    public string PasswordQuestion { get; set; }

    public string PasswordAnswer { get; set; }

    public bool EmailConfirmed { get; set; }

    public string SecurityStamp { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }



    public virtual Address Address1 { get; set; }

    public virtual Division Division1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Transaction> Transactions { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Transaction> Transactions1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<UserClaim> UserClaims { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<UserLogin> UserLogins { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Role> Roles { get; set; }

}

}
