using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ejercicio_Tienda
{
    enum tipoProducto { EXCENTO, BASICO };

    class Tienda
    {
        public static double Iva = 0.10;

        public static void initApp()
        {
            int _opcionMenu;
            int _cantidad;
            int _valor;
            double _precio;
            var _listOpciones = new List<int>() { 1, 2, 3, 4 };
            string _nombreProducto = string.Empty;
            string _tipoProducto = string.Empty;

            List<CarritoCompras> listProductosCompra = new List<CarritoCompras>();
            var listProductos = getProductos();

            cargarOpcionMenu();
            _opcionMenu = obtenerOpcionMenu();

            while (_opcionMenu < 4)
            {
                _nombreProducto = getNombreProducto(_opcionMenu, listProductos);

                _cantidad = obtenerCantidadCapturada(_nombreProducto);

                if (_cantidad != 0)
                {
                    _precio = obtenerPrecioCapturado();

                    _tipoProducto = getTipoProducto(_opcionMenu, listProductos);

                    //carga producto elegido en el carrito de compras
                    listProductosCompra.Add(new CarritoCompras(_nombreProducto, _tipoProducto, _cantidad, _precio));
                }
                else
                {
                    Console.WriteLine("Capture una cantidad mayor a  0 o solo numeros");
                }
                _opcionMenu = obtenerOpcionMenu();
            }

            //Esta linea de codigo simularia el boton de generar venta
            if (_opcionMenu == 4)
            {
                Console.WriteLine(imprimeCompra(listProductosCompra));
            }
            Console.ReadKey();
        }
        static string getNombreProducto(int _identificador, List<Producto> listProductos)
        {
            string _nombreProducto = string.Empty;
            foreach (var item in listProductos)
            {
                if (item.Identificador == _identificador)
                    _nombreProducto = item.NombreProducto;
            }

            return _nombreProducto;
        }

        static string getTipoProducto(int _identificador, List<Producto> listProductos)
        {
            string _tipo = string.Empty;
            foreach (var item in listProductos)
            {
                if (item.Identificador == _identificador)
                    _tipo = item.Tipo;
            }
            return _tipo;
        }

        static string imprimeCompra(List<CarritoCompras> listCarritoCompras)
        {
            string _producto = string.Empty;
            string _resultado = $"\n ------DATOS DE LA COMPRA ------\n\n";
            int _cantidad;
            double _impuestosxProd = 0;
            double _impuestosTotales = 0;
            double _precioSubtotal = 0;
            double _totalVenta = 0;
            List<string> _listProductosAgregados = new List<string>();

            try
            {
                foreach (var item in listCarritoCompras)
                {
                    _producto = item.NombreProducto;

                    if (!_listProductosAgregados.Contains(_producto))
                    {
                        _listProductosAgregados.Add(_producto);

                        _cantidad = (from p in listCarritoCompras where (p.NombreProducto == _producto) select p.Cantidad).Sum();
                        _impuestosxProd = (calculaImpuesto(item.Tipo, item.Precio != null ? item.Precio.Value : 0, _cantidad));
                        _impuestosTotales += _impuestosxProd;
                        _precioSubtotal = (calculaSubtotal(_cantidad, item.Precio != null ? item.Precio.Value : 0));
                        _totalVenta += _precioSubtotal;

                        _resultado += $"{_cantidad} {_producto} { Math.Round((_precioSubtotal + _impuestosxProd), 2)} \n";
                    }
                }

                _resultado += $"Sales taxes: { Math.Round(_impuestosTotales,2)} \n Total: {(Math.Round(_totalVenta + _impuestosTotales, 2)) }";
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.Message);
                throw;
            }
            return _resultado;
        }

        //Este metodo simula la tabla de productos referenciados por el indicador capturado
        static List<Producto> getProductos()
        {

            List<Producto> _list = new List<Producto>();
            Producto _libro = new Producto(1, "Libro", tipoProducto.EXCENTO.ToString());
            Producto _Musica_cd = new Producto(2, "Music CD", tipoProducto.BASICO.ToString());
            Producto _Chocolate_bar = new Producto(3, "Chocolate bar", tipoProducto.EXCENTO.ToString());

            _list.Add(_libro);
            _list.Add(_Musica_cd);
            _list.Add(_Chocolate_bar);

            return _list;
        }

        public class Producto
        {
            public int Identificador { get; set; }
            public string NombreProducto { get; set; }
            public string Tipo { get; set; }

            public Producto(int _identificador, string _nombreProducto, string _tipo)
            {
                Identificador = _identificador;
                NombreProducto = _nombreProducto;
                Tipo = _tipo;
            }
        }

        public class CarritoCompras
        {
            public string NombreProducto { get; set; }
            public string Tipo { get; set; }
            public int Cantidad { get; set; }
            public double? Precio { get; set; }

            public CarritoCompras(string _nombreProducto, string _tipo, int _cantidad, double _precio)
            {
                NombreProducto = _nombreProducto;
                Tipo = _tipo;
                Cantidad = _cantidad;
                Precio = _precio;
            }
        }

        //Este medoto simula una lista de todos los productos en una base de datos, asi como un menu de seleccion para realizar la compra
        static void cargarOpcionMenu()
        {
            Console.WriteLine($"**EJERCICIO PUNTO DE VENTA**  / {DateTime.Now.ToShortDateString()}");
            Console.WriteLine($"1.- Libro");
            Console.WriteLine("2.- Music CD");
            Console.WriteLine("3.- Chocolate bar");
            Console.WriteLine("4.- Realizar compra");
        }

        static int obtenerCantidadCapturada(string _nombreProducto)
        {
            Console.WriteLine($"Producto seleccionado: {_nombreProducto} \n");
            Console.Write("Capture cantidad:");
            return Convert.ToInt32(Console.ReadLine());
        }

        static double obtenerPrecioCapturado()
        {
            string decimales = Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator;
            //En base al separador de decimales, obtengo el de miles
            string miles = decimales == "," ? "." : ",";

            //Aplica el replace
            Console.Write("Capture el precio:");
            double _precio = Convert.ToSingle(Console.ReadLine().Replace(miles, decimales));

            return _precio;
        }

        //Este metodo simula obtener el ID de una tabla de articulos en caso haber seleccionado el articulo
        static int obtenerOpcionMenu()
        {
            Console.Write("\n Agregue al carrito los articulos que desee, elija 4 para generar la compra:");
            return Convert.ToInt32(Console.ReadLine());
        }

        public static double calculaImpuesto(string _tipoProducto, double? _precio, int _cantidad)
        {
            double _impuesto = 0;
            try
            {
                _impuesto = _tipoProducto == tipoProducto.BASICO.ToString() ? (_precio != null ? _precio.Value * Iva : 0 ) * _cantidad : 0;
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.Message);
                throw;
            }
            return _impuesto;
        }

        public static double calculaSubtotal(int _cantidad, double? _precio)
        {
            double _subTotal = 0;
            try
            {
                _subTotal = _precio != null ? _precio.Value * _cantidad : 0;
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.Message);
                throw;
            }
            return _subTotal;
        }
    }
}
