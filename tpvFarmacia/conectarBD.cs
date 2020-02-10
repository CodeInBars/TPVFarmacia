using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace tpvFarmacia
{
    class conectarBD
    {
        MySqlConnection conexion; 
        MySqlCommand comando; 
        MySqlDataReader datos;
        List<claseMedicamento> listaMedicamento = new List<claseMedicamento>();
        List<claseMedicamento> listaMedicamento2 = new List<claseMedicamento>();
        List<claseTarjetaSanitaria> listaTarjetas = new List<claseTarjetaSanitaria>();
        List<claseTratamientos> listaTratamientos = new List<claseTratamientos>();

        public conectarBD()
        {
            //Si Creamos un nuevo objeto se inicia una nueva conexion
            conexion = new MySqlConnection();
            conexion.ConnectionString = "Server=remotemysql.com;Database=Pr1mdxAdrh;Uid=Pr1mdxAdrh;pwd=fNBUrxid1O";

        }
        //Metodo que retorna una lista con objetos de claseMedicamento para listar los medicamentos que hay en la base
        //De datos
        public List<claseMedicamento> listar()
        {
            conexion.Open(); //Abrimos conexion
            String cadenaSql = "select * from medicamento"; //Guardamos la consulta que queremos hacer en un string
            comando = new MySqlCommand(cadenaSql, conexion); //Preparamos la conexion
            datos = comando.ExecuteReader(); //Guardamos los datos obtenidos de la consulta

            while (datos.Read())
            {
                //Mientras haya datos en datos creamos nuevos objetos claseMedicamento
                //y lo añadimos a la listaMedicamento que retornaremos
                claseMedicamento cm = new claseMedicamento();
                cm.Indice = Convert.ToInt16(datos["indice"]);
                cm.Nombre = Convert.ToString(datos["nombre"]);
                cm.Precio = Convert.ToDouble(String.Format("{0:0.00}", datos["precio"]));
                cm.Imagen = (byte[])datos["imagen"];
                cm.Stockactual = Convert.ToInt16(datos["stockactual"]);
                cm.Stockminimo = Convert.ToInt16(datos["stockminimo"]);
                listaMedicamento.Add(cm);
            }
            conexion.Close();
            return listaMedicamento;
        }

        //Metodo para insertar medicamentos en la base de datos
        internal void Insertar(String nombreM, Double precio, byte[] imagen, int stockMin, int stockActual)
        {
            conexion.Open();
            String cadenaSql = "insert into medicamento values(null,?nom,?precio,?imagen,?stockmin,?stockactual)";
            comando = new MySqlCommand(cadenaSql, conexion);
            //Añadimos los parametros a comando 
            comando.Parameters.Add("?nom", MySqlDbType.VarChar).Value = nombreM;
            comando.Parameters.Add("?precio", MySqlDbType.Double).Value = precio;
            comando.Parameters.Add("?imagen", MySqlDbType.Blob).Value = imagen;
            comando.Parameters.Add("?stockmin", MySqlDbType.Int16).Value = stockMin;
            comando.Parameters.Add("?stockActual", MySqlDbType.Int16).Value = stockActual;
            
            comando.ExecuteNonQuery(); //Ejecutamos la conexion
            conexion.Close();
        }

        //Metodo para buscarUsuario en la base de datos, retorna un entero que indica
        //0 El usuario no existe
        //1 El usuario tiene nivel de administrador
        //Otro El usuario tiene nivel de empleado
        public int buscarUsuario(String dni,String pwd)
        {
          
                String sql = "select nivel from usuario where dni=?dni and clave=?pwd "; 
                conexion.Open();
                comando = new MySqlCommand(sql, conexion);
                comando.Parameters.Add("?dni", MySqlDbType.String).Value = dni;
                comando.Parameters.Add("?pwd", MySqlDbType.String).Value = pwd;
                MySqlDataReader datos = comando.ExecuteReader();
                int nivel = 0; //Iniciamos nivel con 0
                if (datos.Read())
                {
                    nivel = Convert.ToInt16(datos["nivel"]);
                }
               
            

            conexion.Close();

            return nivel;
        }

        //Metodo para insertar Facturas en la base de datos
        public void insertarFacturas(List<claseMedicamento> listaCesta, string dniVendedor, double total)
        {
            string cadenaProductos = ""; //Creamos un string en el que añadiremos todos los nombres de los medicamentos de la listaCesta
            for (int i = 0; i < listaCesta.Count; i++)
            {
                cadenaProductos += listaCesta[i].Nombre + ",";
            }
            conexion.Open();
            String cadenaSql="insert into facturacion values(null,?dni,?cadenaProd,?fecha,?total)"; //Creamos la consulta
            comando = new MySqlCommand(cadenaSql, conexion);
            //Añadimos los parametros
            comando.Parameters.Add("?dni", MySqlDbType.VarChar).Value = dniVendedor;
            comando.Parameters.Add("?cadenaProd", MySqlDbType.VarChar).Value = cadenaProductos;
            comando.Parameters.Add("?fecha", MySqlDbType.DateTime).Value = DateTime.Now;
            comando.Parameters.Add("?total", MySqlDbType.Double).Value = total;
            comando.ExecuteNonQuery();
            conexion.Close();
            
        }

        //Metodo para modificar medicamentos de la base de datos a partir del indice
        internal void modificarMedicamento(claseMedicamento med)
        {
            conexion.Open();
            String cadenaSql = "update  medicamento set nombre=?nom,precio=?pr,stockminimo=?sm,stockactual=?sa,imagen=?im where indice=?id";
            comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?id", MySqlDbType.Int16).Value = med.Indice;
            comando.Parameters.Add("?nom", MySqlDbType.VarChar).Value = med.Nombre;
            comando.Parameters.Add("?pr", MySqlDbType.Double).Value = med.Precio;

            comando.Parameters.Add("?sa", MySqlDbType.Int16).Value = med.Stockactual;
            comando.Parameters.Add("?sm", MySqlDbType.Int16).Value = med.Stockminimo;
            comando.Parameters.Add("?im", MySqlDbType.Blob).Value = (byte[])med.Imagen;
            comando.ExecuteNonQuery();
            conexion.Close();
        }

        //Metodo para restar stock de un medicamento
        public void Lanzar_actualizacion(List<claseMedicamento> listaCesta)
        {
            conexion.Open();
            for (int i = 0; i < listaCesta.Count; i++)
            {
                string NombreMed = listaCesta[i].Nombre; //Guardamos el nombre de un medicamento de listaCesta en un string
                String cadenaSql = "update medicamento set stockactual=stockactual-1 where nombre= '" + NombreMed+"'";
                comando = new MySqlCommand(cadenaSql, conexion);
                comando.ExecuteNonQuery();
            }
            
            conexion.Close();
        }

        //Metodo para listar las tarjetas sanitarias de la base de datos
        public List<claseTarjetaSanitaria> listarTarjetas()
        {
            conexion.Open();
            String cadenaSql = "select * from tarjetaSanitaria";
            comando = new MySqlCommand(cadenaSql, conexion);
            datos = comando.ExecuteReader();
            while (datos.Read())
            {
                //Mientras haya datos lo metemos en un objeto de claseTarjetaSanitaria y lo guardamos en
                //listaTarjetas, despues retornamos esa lista

                claseTarjetaSanitaria cTS = new claseTarjetaSanitaria();
                cTS.Dni = Convert.ToString(datos["dni"]);
                cTS.Nombre = Convert.ToString(datos["nombre"]);
                cTS.Email = Convert.ToString(datos["email"]);
                cTS.FechaNacimiento = Convert.ToDateTime(datos["fechaNacimiento"]);
                listaTarjetas.Add(cTS);
            }
            conexion.Close();
            return listaTarjetas;

        }

        //Metodo para listar tratamientos de un dni
        public List<claseTratamientos> listarTratamientos(string dni, int mes)
        {
            conexion.Open();
            //Si el dni coincide y recogido es igual a 0
            String cadenaSql = "select * from tratamientos where dni=?d and mes=?m and recogido=0";
            comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?d", MySqlDbType.VarChar).Value = dni;
            comando.Parameters.Add("?m", MySqlDbType.Int16).Value = mes;
            datos = comando.ExecuteReader();
            while (datos.Read())
            {
                //Mientras haya datos metemos los datos en un objeto de claseTratamientos y lo metemos en listaTratamientos y la retornamos
                claseTratamientos cT = new claseTratamientos();
                cT.Identificador = Convert.ToInt16(datos["identificador"]);
                cT.Dni = Convert.ToString(datos["dni"]);
                cT.Medicamento = Convert.ToString(datos["medicamento"]);
                cT.Mes = Convert.ToInt16(datos["mes"]);
                cT.Recogido = Convert.ToInt16(datos["recogido"]);

                listaTratamientos.Add(cT);
            }
            conexion.Close();
            return listaTratamientos;
        }

        //Metodo para decir que el cliente a recogido el medicamento
        public void actualizarTratamiento(List<claseMedicamento> listaCesta, string dni,int mes)
        {
          conexion.Open();
          for (int i = 0; i < listaCesta.Count; i++)
            {
                //Metemos una consulta de update donde ponemos que recojido es igual a 1 para que no pueda volver a recojer el medicamento
                //Los medicamentos los recojemos de listaCesta
                String cadenaSql = "update tratamientos set recogido=1 where dni=?d and medicamento=?m and mes=?mes";
                comando = new MySqlCommand(cadenaSql, conexion);
                comando.Parameters.Add("?d", MySqlDbType.VarChar).Value = dni;
                comando.Parameters.Add("?m", MySqlDbType.VarChar).Value = listaCesta[i].Nombre;
                comando.Parameters.Add("?mes", MySqlDbType.Int16).Value = mes;
                comando.ExecuteNonQuery();
            }
            conexion.Close();
        }

        //Lista los medicamentos por debajo del stock
        public List<claseMedicamento> listarMinimo()
        {
            conexion.Open();
            String cadenaSql = "Select * from medicamento where stockactual<stockminimo";
            comando = new MySqlCommand(cadenaSql, conexion);
            datos = comando.ExecuteReader();
            while (datos.Read()) {
                //Mientras haya datos en datos creamos nuevos objetos claseMedicamento
                //y lo añadimos a la listaMedicamento que retornaremos
                claseMedicamento cm = new claseMedicamento();
                cm.Indice = Convert.ToInt16(datos["indice"]);
                cm.Nombre = Convert.ToString(datos["nombre"]);
                cm.Precio = Convert.ToDouble(String.Format("{0:0.00}", datos["precio"]));
                cm.Imagen = (byte[])datos["imagen"];
                cm.Stockactual = Convert.ToInt16(datos["stockactual"]);
                cm.Stockminimo = Convert.ToInt16(datos["stockminimo"]);
                listaMedicamento2.Add(cm);
            }
            conexion.Close();
            return listaMedicamento2;
        }

        //Metodo para encontrar un medicamento con el indicador
        public claseMedicamento buscarmedicamento(String barIdentificador) {

            conexion.Open();
            claseMedicamento cm = new claseMedicamento();
            String cadenaSql = "Select * from medicamento where indice=?var";
            comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?var", MySqlDbType.Int16).Value = barIdentificador;
            datos = comando.ExecuteReader();
            while (datos.Read()) {
                cm = new claseMedicamento();
                cm.Indice = Convert.ToInt16(datos["indice"]);
                cm.Nombre = Convert.ToString(datos["nombre"]);
                cm.Precio = Convert.ToDouble(String.Format("{0:0.00}", datos["precio"]));
                cm.Imagen = (byte[])datos["imagen"];
                cm.Stockactual = Convert.ToInt16(datos["stockactual"]);
                cm.Stockminimo = Convert.ToInt16(datos["stockminimo"]);
            }
            conexion.Close();
            return cm;
        }

        //Añadir mas 1 en el stockactual de un medicamento
        public void añadirMedicamento(claseMedicamento cm) {

            conexion.Open();
            String cadenaSql = "Update medicamento set stockactual = (stockactual+1) where indice=?in";
            comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?in", MySqlDbType.Int16).Value = cm.Indice;
            comando.ExecuteNonQuery();

            conexion.Close();
        }

        public void exbackup() {

            string nombreFichero = "C:\\backup\\backup.sql";
            conexion.Open();
            comando = new MySqlCommand();
            MySqlBackup mb = new MySqlBackup(comando);

            comando.Connection = conexion;
            
            mb.ExportToFile(nombreFichero);
            conexion.Close();
        }

        public void inbackup() {

            String nombreFichero = "C:\\backup\\backup.sql";

            conexion.Open();
            comando = new MySqlCommand();
            MySqlBackup mb = new MySqlBackup(comando);
            
            comando.Connection = conexion;
            mb.ImportFromFile(nombreFichero);
            conexion.Close();
            
        }
    }
}
