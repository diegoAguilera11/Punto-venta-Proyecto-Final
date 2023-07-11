using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;

string fechaVenta;
string fechaLocal;

// MODIFICAR
string archivoInventario = @"C:\Users\diego\Desktop\Cajero\productos.json";
string productos = File.ReadAllText(archivoInventario);

string archivoCajas = @"C:\Users\diego\Desktop\Cajero\caja.json";
string cajas = File.ReadAllText(archivoCajas);

List<ProductoInventario> productosInventario = JsonConvert.DeserializeObject<
    List<ProductoInventario>
>(productos);

List<Caja> cajasInventario = JsonConvert.DeserializeObject<List<Caja>>(cajas);

// Listas que almacenan las sucursales y categorias manejadas por el inventario
List<Sucursal> sucursales = new List<Sucursal>();
List<Categoria> categorias = new List<Categoria>();

// Lista la cual almacenara solamente los productos de la sucursal seleccionada.
List<ProductoInventario> productosSucursal = new List<ProductoInventario>();

//OBTIENE LA FECHA LOCAL
DateTime fechaHoraActual = DateTime.Now;
TimeSpan horaActual = fechaHoraActual.TimeOfDay;
fechaLocal =
    fechaHoraActual.Day.ToString()
    + fechaHoraActual.Month.ToString()
    + fechaHoraActual.Year.ToString();

void lecturaSucursales()
{
    foreach (var item in productosInventario)
    {
        Sucursal sucursalEncontrada = sucursales.Find(
            sucursal => item.sucursal_id == sucursal.idsucursal
        );
        if (sucursalEncontrada == null)
        {
            sucursales.Add(
                new Sucursal { idsucursal = item.sucursal_id, nombre = item.nombre_sucursal }
            );
        }
    }
}

void lecturaCategorias()
{
    foreach (var item in productosInventario)
    {
        Categoria categoriaEncontrada = categorias.Find(
            categoria => item.categoria_id == categoria.idcategoria
        );
        if (categoriaEncontrada == null)
        {
            categorias.Add(
                new Categoria { idcategoria = item.categoria_id, nombre = item.nombre_categoria }
            );
        }
    }
}

void mostrarProductos(string sucursalIngresada)
{
    System.Console.WriteLine("LISTADO DE PRODUCTOS DE LA SUCURSAL " + sucursalIngresada.ToUpper());
    System.Console.WriteLine("-------------------------------------------------------------------");
    foreach (var producto in productosInventario)
    {
        if (producto.nombre_sucursal == sucursalIngresada)
        {
            System.Console.WriteLine(producto.ToString());
            System.Console.WriteLine("------------------");
            productosSucursal.Add(
                new ProductoInventario
                {
                    producto_id = producto.producto_id,
                    codigo = producto.codigo,
                    nombre = producto.nombre,
                    categoria_id = producto.categoria_id,
                    nombre_categoria = producto.nombre_categoria,
                    sucursal_id = producto.sucursal_id,
                    nombre_sucursal = producto.nombre_sucursal,
                    precio = producto.precio,
                    cantidad = producto.cantidad
                }
            );
        }
    }
}

void compraProducto(ProductoInventario producto, List<ProductoJSON> productosCliente)
{
    bool numeroValido = false;
    var cantidadComprada = "";

    do
    {
        Console.WriteLine("Indique la cantidad a comprar del producto: " + producto.nombre);
        cantidadComprada = Console.ReadLine();
        numeroValido = validarNumero(cantidadComprada);
    } while (!numeroValido);

    System.Console.WriteLine(producto);

    // Se agrega a la lista de productos comprados.
    productosCliente.Add(
        new ProductoJSON
        {
            producto_id = producto.producto_id,
            cantidad = int.Parse(cantidadComprada),
            precio = producto.precio,
            categoria_id = producto.categoria_id,
            nombre_categoria = producto.nombre_categoria
        }
    );
}

int calcularTotalVenta(List<ProductoJSON> productosCliente)
{
    int total_venta = 0;
    foreach (var item in productosCliente)
    {
        total_venta += item.precio * item.cantidad;
    }

    return total_venta;
}

void realizarVenta(
    int idCaja,
    int numeroCaja,
    int idSucursalIngresada,
    string sucursalIngresada,
    string rutClienteActual
)
{
    List<ProductoJSON> productosCliente = new List<ProductoJSON>();

    Console.WriteLine("Ingrese el código del producto, para finalizar ingrese 8888");
    string codigoProducto = Console.ReadLine();

    while (codigoProducto != "8888")
    {
        ProductoInventario productoEncontrado = productosSucursal.Find(
            producto => codigoProducto == producto.codigo
        );

        while (productoEncontrado == null)
        {
            Console.WriteLine("Ingrese el código del producto, para finalizar ingrese 8888");
            codigoProducto = Console.ReadLine();

            productoEncontrado = productosSucursal.Find(
                producto => codigoProducto == producto.codigo
            );
        }

        compraProducto(productoEncontrado, productosCliente);

        Console.WriteLine("Ingrese el código del producto, para finalizar ingrese 8888");
        codigoProducto = Console.ReadLine();
    }

    fechaHoraActual = DateTime.Now;
    horaActual = fechaHoraActual.TimeOfDay;
    fechaVenta =
        fechaHoraActual.Day.ToString()
        + fechaHoraActual.Month.ToString()
        + fechaHoraActual.Hour.ToString()
        + fechaHoraActual.Minute.ToString()
        + fechaHoraActual.Second.ToString();

    string correlativoVenta = (numeroCaja + fechaVenta).ToString();

    // Generar venta
    Venta venta = new Venta
    {
        sucursal_id = idSucursalIngresada,
        nombre_sucursal = sucursalIngresada,
        correlativo = correlativoVenta,
        fecha = fechaVenta,
        rutCliente = rutClienteActual,
        total_venta = calcularTotalVenta(productosCliente),
        caja_id = idCaja,
        productosVenta = productosCliente
    };

    // Conexión rabbit, enviar la venta creada.




    // desplegarBoleta();
}

bool validarNumero(string valor)
{
    try
    {
        int.Parse(valor);
        return true;
    }
    catch (FormatException)
    {
        return false;
    }
}

int validarCaja(int caja, int idSucursalIngresada)
{

    Caja cajaEncontrada = cajasInventario.Find(
        cajaInventario =>
            cajaInventario.numero == caja && cajaInventario.sucursal_id == idSucursalIngresada
    );
    if (cajaEncontrada == null)
    {
        return -1;
    }

    return cajaEncontrada.caja_id;
}

lecturaSucursales();
lecturaCategorias();

// PREGUNTAR LA SUCURSAL QUE SE ENCUENTRA EL CLIENTE
bool existe = false;
string sucursalIngresada;
int idSucursalIngresada = 0;
do
{
    Console.WriteLine("Ingrese la sucursal en la que se encuentra: ");
    sucursalIngresada = Console.ReadLine();

    Sucursal sucursalBuscada = sucursales.Find(
        sucursal => sucursalIngresada == sucursal.nombre.ToString()
    );
    if (sucursalBuscada != null)
    {
        idSucursalIngresada = sucursalBuscada.idsucursal;
        existe = true;
    }
} while (!existe);

bool numeroValido = false;
int cajaValida = -1;
string numeroCajaAux;
int intentos = 0;
do
{
    if (intentos == 0)
    {
        Console.WriteLine("Bienvenido a " + sucursalIngresada + " ingrese el número de caja: ");
    }
    else
    {
        Console.WriteLine("ERROR, ingrese nuevamente el número de caja: ");
    }
    numeroCajaAux = Console.ReadLine();
    numeroValido = validarNumero(numeroCajaAux);
    if (numeroValido)
    {
        cajaValida = validarCaja(int.Parse(numeroCajaAux), idSucursalIngresada);
    }
    intentos += 1;
} while (!numeroValido || cajaValida == -1);

int numeroCaja = int.Parse(numeroCajaAux);
int idCaja = cajaValida;

Console.WriteLine("Indique el RUT del cliente, 777 para terminar dia de ventas");
string rutClienteActual = Console.ReadLine();

while (rutClienteActual != "777")
{
    mostrarProductos(sucursalIngresada);

    realizarVenta(idCaja, numeroCaja, idSucursalIngresada, sucursalIngresada, rutClienteActual);

    Console.WriteLine("Indique el RUT del cliente, 777 para terminar dia de ventas");
    rutClienteActual = Console.ReadLine();
}
