
public class ProductoInventario
{
    public int producto_id { get; set; }
    public string codigo { get; set; }
    public string nombre { get; set; }
    public int categoria_id { get; set; }
    public string nombre_categoria { get; set; }
    public int sucursal_id { get; set; }
    public string nombre_sucursal { get; set; }
    public int precio { get; set; }
    public int cantidad { get; set; }

    public override string ToString()
    {
        return $"Código: {codigo} - Nombre: {nombre} - Precio: $ {precio} - Cantidad disponible: {cantidad}";
    }
}
