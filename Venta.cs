public class Venta
{
    public int sucursal_id { get; set; }
    public string nombre_sucursal { get; set; }
    public string correlativo { get; set; }
    public string fecha { get; set; }
    public string rutCliente { get; set; }
    public int total_venta { get; set; }
    public int caja_id { get; set; }

    public List<ProductoJSON> productosVenta { get; set; }

    public override string ToString()
    {
        return $"Sucursal id: {sucursal_id} - Nombre sucursal: {nombre_sucursal} - Correlativo: {correlativo} - Fecha: {fecha} - Total: {total_venta} - Id caja: {caja_id} - RUT Cliente: {rutCliente} ";
    }
}
