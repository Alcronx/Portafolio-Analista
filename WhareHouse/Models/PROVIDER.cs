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
    
    public partial class PROVIDER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PROVIDER()
        {
            this.PRODUCT = new HashSet<PRODUCT>();
        }
    
        public byte IDPROVIDER { get; set; }
        public string RUT { get; set; }
        public string COMPANYNAME { get; set; }
        public string NAME1 { get; set; }
        public string NAME2 { get; set; }
        public string LASTNAME1 { get; set; }
        public string LASTNAME2 { get; set; }
        public string REGION { get; set; }
        public string COMMUNE { get; set; }
        public string DIRECTION { get; set; }
        public string COMPANYITEM { get; set; }
        public long CELLPHONE { get; set; }
        public string MAIL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCT> PRODUCT { get; set; }
    }
}