using System.ComponentModel.DataAnnotations;


namespace Entity.Enum
{
    public enum TypeAction
    {
        [Display(Name = "Venta Realizada")]
        Sale = 1,

        [Display(Name = "Compra Realizada")]
        Buy = 2,

        [Display(Name = "Movimiento En Inventario Realizado")]
        Movement = 3,
    }
}