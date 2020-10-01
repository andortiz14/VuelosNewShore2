using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class BaseDatos
    {
        private SqlConnection conexion = null;
        private SqlCommand comando = null;
        private SqlTransaction transaccion = null;
        private SqlDataAdapter adaptador = null;
        private string cadenaConexion;
        private string _connectionString;
        private bool m_isOpen = false;
        private bool m_isInTransaction = false;
        private bool m_isInMultipleOperations = false;

        private bool m_isCorrectTransaction = false;
        public bool IsCorrectTransaction
        {
            get { return m_isCorrectTransaction; }
            set { m_isCorrectTransaction = value; }
        }

        public bool IsInMultipleOperations
        {
            get { return m_isInMultipleOperations; }
            set { m_isInMultipleOperations = value; }
        }

        public bool IsInTransaction
        {
            get { return m_isInTransaction; }
            set { m_isInTransaction = value; }
        }

        public bool IsOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }

        /// <summary>
        /// Establece u obtiene el tiempo de espera que tiene establecido un comando para ejecutarse
        /// </summary>
        public int TimeOut
        {
            get { return this.comando.CommandTimeout; }
            set { this.comando.CommandTimeout = value; }
        }

        /// <summary>
        /// Crea una instancia del acceso a la base de datos.
        /// </summary>
        public BaseDatos()
        {
            Configurar();
        }

        private static DbProviderFactory factory = null;

        /// <summary>
        /// Configura el acceso a la base de datos para su utilización.
        /// </summary>
        /// <exception cref="BaseDatosException">Si existe un error al cargar la configuración.</exception>
        private void Configurar()
        {
            try
            {
                this.cadenaConexion = ConfigurationManager.AppSettings.Get("CADENA_CONEXION");
              
            }
            catch (ConfigurationException ex)
            {
                throw new BaseDatosException("Error al cargar la configuración del acceso a datos.", ex);
            }
        }

        protected string ConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connectionString))
                {

                    _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// Permite desconectarse de la base de datos.
        /// </summary>
        public void Close()
        {
            if (this.conexion != null && this.conexion.State.Equals(ConnectionState.Open))
            {
                this.conexion.Close();
                m_isOpen = false;
            }
        }

        /// <summary>
        /// Se concecta con la base de datos.
        /// </summary>
        /// <exception cref="BaseDatosException">Si existe un error al conectarse.</exception>
        public void Open()
        {
            if (this.conexion != null && !this.conexion.State.Equals(ConnectionState.Closed))
            {
                throw new BaseDatosException("La conexión ya se encuentra abierta.");
            }
            try
            {
                if (this.conexion == null)
                {
                    this.conexion = new SqlConnection();
                    this.conexion.ConnectionString = this.cadenaConexion;
                }
                this.conexion.Open();
                m_isOpen = true;
            }
            catch (DataException ex)
            {
                throw new BaseDatosException("Error al conectarse a la base de datos.", ex);
            }
        }

        public void OpenSec()
        {
            //Se Conecta a la DB de Seguridad
            _connectionString = ConnectionString.ToString();
            if (this.conexion != null && !this.conexion.State.Equals(ConnectionState.Closed))
            {
                throw new BaseDatosException("La conexión ya se encuentra abierta.");
            }
            try
            {
                if (this.conexion == null)
                {
                    this.conexion = new SqlConnection();
                    this.conexion.ConnectionString = this._connectionString;
                }
                this.conexion.Open();
                m_isOpen = true;
            }
            catch (DataException ex)
            {
                throw new BaseDatosException("Error al conectarse a la base de datos.", ex);
            }
        }

        /// <summary>
        /// Crea un comando en base a un store procedure de SQL.
        /// Ejemplo:
        /// <code>OBTIENE_DIVISION</code>
        /// Guarda el store para el seteo de parámetros y la posterior ejecución.
        /// </summary>
        /// <param name="storeProcedure">Nombre del store procedure de SQL</param>
        public void CreateCommand(string storeProcedure)
        {
            this.comando = new System.Data.SqlClient.SqlCommand();
            this.comando.Connection = this.conexion;
            this.comando.CommandTimeout = 0;
            this.comando.CommandType = CommandType.StoredProcedure;
            this.comando.CommandText = storeProcedure;
            if (this.transaccion != null)
            {
                this.comando.Transaction = this.transaccion;
            }
        }

        /// <summary>
        /// Reasigna al comando el commandText en base a un store de Sql
        /// Limpia los parámetros para poder reasignarlos
        /// </summary>
        /// <param name="storeProcedure">Nombre del nuevo store</param>
        public void ReasignarComando(string storeProcedure)
        {
            if (this.comando != null)
            {
                this.comando.CommandText = storeProcedure;
                this.comando.Parameters.Clear();
            }
            else
            {
                throw new Exception("No existe un comando creado");
            }
        }

        /// <summary>
        /// Crea un comando en base a un query.
        /// Ejemplo:
        /// <code>SELECT * FROM tabla WHERE campo=@valor</code>
        /// Guarda query para el seteo de parámetros y la posterior ejecución.
        /// </summary>
        /// <param name="query">Sentencia SELECT de SQL</param>
        public void CrearQuery(string query)
        {
            this.comando = new System.Data.SqlClient.SqlCommand();
            this.comando.Connection = this.conexion;
            this.comando.CommandType = CommandType.Text;
            this.comando.CommandText = query;
            if (this.transaccion != null)
            {
                this.comando.Transaction = this.transaccion;
            }
        }

        /// <summary>
        /// Setea un parámetro como nulo del store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro cuyo valor será nulo.</param>
        /// <param name="tipo">El nombre de valor del parámetro cuyo valor será nulo.</param>
        public void CreateParameter(string nombre, SqlDbType tipo)
        {
            if (tipo == SqlDbType.NVarChar)
            {
                this.CreateParameters(nombre, tipo, null);
            }
            else
            {
                this.CreateParameters(nombre, tipo, DBNull.Value);
            }
        }

        /// <summary>
        /// Asigna un parámetro de tipo cadena al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, string valor)
        {
            if (valor == null)
            {
                this.CreateParameters(nombre, SqlDbType.NVarChar, DBNull.Value);
            }
            else
            {
                this.CreateParameters(nombre, SqlDbType.NVarChar, valor);
            }
        }

        /// <summary>
        /// Asigna un parámetro de tipo entero al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, int valor)
        {
            this.CreateParameters(nombre, SqlDbType.Int, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo entero nullable al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, Nullable<int> valor)
        {
            if (valor == null)
            {
                this.CreateParameters(nombre, SqlDbType.Int, DBNull.Value);
            }
            else
            {
                this.CreateParameters(nombre, SqlDbType.Int, valor);
            }
        }

        /// <summary>
        /// Asigna un parámetro de tipo entero al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, long valor)
        {
            this.CreateParameters(nombre, SqlDbType.BigInt, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo entero nullable al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, Nullable<long> valor)
        {
            if (valor == null)
            {
                this.CreateParameters(nombre, SqlDbType.BigInt, DBNull.Value);
            }
            else
            {
                this.CreateParameters(nombre, SqlDbType.BigInt, valor);
            }
        }
        /// <summary>
        /// Asigna un parámetro de tipo fecha al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, DateTime valor)
        {
            this.CreateParameters(nombre, SqlDbType.DateTime, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo fecha nullable al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, Nullable<DateTime> valor)
        {
            if (valor == null)
            {
                this.CreateParameters(nombre, SqlDbType.DateTime, DBNull.Value);
            }
            else
            {
                this.CreateParameters(nombre, SqlDbType.DateTime, valor);
            }
        }

        /// <summary>
        /// Asigna un parámetro de tipo UniqueIdentifier al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, Guid valor)
        {
            this.CreateParameters(nombre, SqlDbType.UniqueIdentifier, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo doble al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, double valor)
        {
            this.CreateParameters(nombre, SqlDbType.Float, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo doble nullable al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, Nullable<double> valor)
        {
            if (valor == null)
            {
                this.CreateParameters(nombre, SqlDbType.Float, DBNull.Value);
            }
            else
            {
                this.CreateParameters(nombre, SqlDbType.Float, valor);
            }
        }

        /// <summary>
        /// Asigna un parámetro de tipo booleano al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, bool valor)
        {
            this.CreateParameters(nombre, SqlDbType.Bit, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo booleano nullable al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, Nullable<bool> valor)
        {
            if (valor == null)
            {
                this.CreateParameters(nombre, SqlDbType.Bit, DBNull.Value);
            }
            else
            {
                this.CreateParameters(nombre, SqlDbType.Bit, valor);
            }
        }

        /// <summary>
        /// Asigna un parámetro de tipo arreglo de bytes al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, byte[] valor)
        {
            this.CreateParameters(nombre, SqlDbType.Image, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo lista que herede de IEnumarable al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, DataTable valor)
        {
            this.CreateParameters(nombre, SqlDbType.Structured, valor);
        }

        /// <summary>
        /// Crea un parámetro y asigna su valor para el comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="tipo">El tipo de dato del parámetro que será creado.</param>
        /// <param name="valor">El valor que se asignará al parámetro.</param>
        private void CreateParameters(string nombre, SqlDbType tipo, object valor)
        {
            if (!this.comando.Parameters.Contains(nombre))
            {
                this.comando.Parameters.Add(nombre, tipo).Value = valor;
            }
            else
            {
                this.comando.Parameters[nombre].Value = valor;
            }
        }

        /// <summary>
        /// Asigna un parámetro de tipo Numeric al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, decimal valor)
        {
            this.CreateParameters(nombre, SqlDbType.Money, valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo Numeric nullable al store procedure creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void CreateParameter(string nombre, Nullable<decimal> valor)
        {
            if (valor == null)
            {
                this.CreateParameters(nombre, SqlDbType.Money, DBNull.Value);
            }
            else
            {
                this.CreateParameters(nombre, SqlDbType.Money, valor);
            }
        }

        /// <summary>
        /// Ejecuta el store procedure creado y retorna el resultado de la consulta.
        /// </summary>
        /// <returns>El resultado de la consulta.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el comando.</exception>
        public DbDataReader EjecutarConsulta()
        {
            return this.comando.ExecuteReader();
        }

        /// <summary>
        /// Ejecuta el store procedure creado y retorna el resultado de la consulta.
        /// </summary>
        /// <param name="nombreTabla">Nombre de la tabla creada</param>
        /// <returns>El resultado de la consulta en una tabla.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el store</exception>
        public DataTable EjecutarConsulta(string nombreTabla)
        {
            DataTable tabla = new DataTable(nombreTabla);
            adaptador = new SqlDataAdapter(this.comando);
            try
            {
                adaptador.Fill(tabla);
            }
            catch (InvalidOperationException ex)
            {
                throw new BaseDatosException("Error al poblar la tabla. ", ex);
            }
            return tabla;
        }

        /// <summary>
        /// Ejecuta el store procedure creado y retorna el resultado de mas de una consulta.
        /// </summary>
        /// <param name="nombreDataSet">nombre del DataSet creado</param>
        /// <returns>El resultado de varias consultas en un DataSet</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el store</exception>
        public DataSet EjecutarDataSet(string nombreDataSet)
        {
            DataSet dataSet = new DataSet(nombreDataSet);
            adaptador = new SqlDataAdapter(this.comando);
            try
            {
                adaptador.Fill(dataSet);
            }
            catch (InvalidOperationException ex)
            {
                throw new BaseDatosException("Error al poblar la tabla. ", ex);
            }
            return dataSet;
        }

        /// <summary>
        /// Ejecuta el store procedure creado y retorna el valor de la primera columna del primer registro.
        /// </summary>
        /// <returns>El valor que es el resultado del comando.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el store.</exception>
        public object EjecutarEscalar()
        {
            object escalar = null;
            try
            {
                escalar = this.comando.ExecuteScalar();
            }
            catch (InvalidCastException ex)
            {
                throw new BaseDatosException("Error al ejecutar un escalar.", ex);
            }
            return escalar;
        }
        /// <summary>
        /// Ejecuta una instrucción de Transact-SQL en la conexión y devuelve el número de filas afectadas.
        /// </summary>
        //// <returns>El valor que es el resultado del comando.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el store.</exception>
        public int ExecuteNonQuery()
        {
            return this.comando.ExecuteNonQuery();
        }

        /// <summary>
        /// Ejecuta el store procedure creado.
        /// </summary>
        public void EjecutarComando()
        {
            this.comando.ExecuteNonQuery();
        }

        /// <summary>
        /// Comienza una transacción en base a la conexion abierta.
        /// Todo lo que se ejecute luego de esta ionvocación estará 
        /// dentro de una tranasacción.
        /// </summary>
        public void ComenzarTransaccion()
        {
            if (this.transaccion == null)
            {
                string nivel = ConfigurationManager.AppSettings["AislamientoBD"];
                switch (nivel)
                {
                    case "1":
                        this.transaccion = this.conexion.BeginTransaction(IsolationLevel.ReadCommitted);
                        break; // TODO: might not be correct. Was : Exit Select

                    case "2":
                        this.transaccion = this.conexion.BeginTransaction(IsolationLevel.ReadUncommitted);
                        break; // TODO: might not be correct. Was : Exit Select

                    case "3":
                        this.transaccion = this.conexion.BeginTransaction(IsolationLevel.RepeatableRead);
                        break; // TODO: might not be correct. Was : Exit Select

                    case "4":
                        this.transaccion = this.conexion.BeginTransaction(IsolationLevel.Serializable);
                        break; // TODO: might not be correct. Was : Exit Select

                    case "5":
                        this.transaccion = this.conexion.BeginTransaction(IsolationLevel.Snapshot);
                        break; // TODO: might not be correct. Was : Exit Select

                    case "6":
                        this.transaccion = this.conexion.BeginTransaction(IsolationLevel.Chaos);
                        break; // TODO: might not be correct. Was : Exit Select
                    case "7":
                        this.transaccion = this.conexion.BeginTransaction(IsolationLevel.Unspecified);
                        break; // TODO: might not be correct. Was : Exit Select
                    default:
                        this.transaccion = this.conexion.BeginTransaction();
                        break; // TODO: might not be correct. Was : Exit Select
                }
                m_isInTransaction = true;
            }
        }

        /// <summary>
        /// Cancela la ejecución de una transacción.
        /// Todo lo ejecutado entre ésta invocación y su 
        /// correspondiente <c>ComenzarTransaccion</c> será perdido.
        /// </summary>
        public void CancelarTransaccion()
        {
            if (this.transaccion != null)
            {
                this.transaccion.Rollback();
                m_isInTransaction = false;
            }
        }

        /// <summary>
        /// Confirma todo los comandos ejecutados entre el <c>ComanzarTransaccion</c>
        /// y ésta invocación.
        /// </summary>
        public void ConfirmarTransaccion()
        {
            if (this.transaccion != null)
            {
                this.transaccion.Commit();
                m_isInTransaction = false;
            }
        }

    }
}
