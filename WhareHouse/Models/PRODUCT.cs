//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WhareHouse.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRODUCT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUCT()
        {
            this.EXPIRATIONDATE = new HashSet<EXPIRATIONDATE>();
            this.ORDERDETAILS = new HashSet<ORDERDETAILS>();
            this.TICKETDETAILS = new HashSet<TICKETDETAILS>();
        }
    
        public short IDBARCODE { get; set; }
        public long BARCODE { get; set; }
        public int  PURCHASEPRICE { get; set; }
        public int SALEPRICE { get; set; }
        public short STOCK { get; set; }
        public byte CRITICALSTOCK { get; set; }
        public string PRODUCTNAME { get; set; }
        public string PRODUCTFAMILY { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string PRODUCTDESCRIPTION { get; set; }
        public string STATE { get; set; }
        public short IDPROVIDER { get; set; }
        
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXPIRATIONDATE> EXPIRATIONDATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDERDETAILS> ORDERDETAILS { get; set; }
        public virtual PROVIDER PROVIDER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TICKETDETAILS> TICKETDETAILS { get; set; }
    }
}
