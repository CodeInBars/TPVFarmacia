using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using MessagingToolkit.Barcode;
using System.Net.Mail;
using System.Net;

namespace tpvFarmacia
{
    public partial class Form1 : Form
    {
        conectarBD cnx; //Instanciamos un objeto De la clase conectarBD
        List<claseMedicamento> listaMedicamento = new List<claseMedicamento>(); //Lista para guardar todos los medicamentos de la base de datos
        List<claseMedicamento> listaCesta = new List<claseMedicamento>(); //Lista para guardar todos los medicamentos que se han seleccionado
        List<claseMedicamento> MedEncontrados = new List<claseMedicamento>(); //Lista para guardar los medicamentos al usar el buscador
        List<claseTarjetaSanitaria> listaTarjetas = new List<claseTarjetaSanitaria>(); //Lista para guardar las tarjetas
        List<claseTratamientos> listaTratamientos = new List<claseTratamientos>(); //Lista para guardar los tratamientos

        double total = 0; //Guarda el precio total de todos los medicamentos seleccionados
        String pdfTicket; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            downAll();
            dataGridView1.Rows.Add("Total :", total); //Añadimos al dataGridView1 una columna con el total
            cnx = new conectarBD(); 
            listaMedicamento = cnx.listar(); //Llenamos la listaMedicamento con el resultado del Metodo listar
            cargarTPV(listaMedicamento); //Llamamos al metodo cargarTPV y le pasamo la listaMedicamento

        }

        //Metodo para añadir todos los medicamentos de la base de datos
        private void cargarTPV(List<claseMedicamento> listaMedicamento)
        {
            //Guardamos las filas y columnas que queremos que haya en el tabControl mediante los numericUpDown1
            //Y calculamos el numero de pantallas que habra
            int nfilas = Convert.ToInt16(numericUpDown1.Value);
            int ncolumnas= Convert.ToInt16(numericUpDown2.Value);
            int nPantallas = listaMedicamento.Count / (nfilas*ncolumnas);

            //construir un tabpage cada x medicamentos
         
            int indiceLista = 0; //Para dar un indice a cada boton y saber el indice de la lista
            int ancho = tabControl1.Width-40; //Para dar anchura a los botones 
            int alto = tabControl1.Height-40; //Para dar altura a los botones 

            for (int indicePanales = 0; indicePanales <= nPantallas; indicePanales++)
            {
                //Creamos una nueva pagina para nuestra tabControl
                TabPage tp = new TabPage(Convert.ToString(indicePanales + 1));
                tabControl1.Controls.Add(tp); //Añadimos al tabControl la pagina

                TableLayoutPanel tlp = new TableLayoutPanel();
                tlp.AutoSize = true;
                tlp.RowCount = nfilas ;
                tlp.ColumnCount = ncolumnas;
                tp.Controls.Add(tlp);

                for (int contMed = 0; contMed < nfilas * ncolumnas; contMed++)
                {
                    //construir y diseña el botón
                  
                        Button botonX = new Button();
                        botonX.Height =alto/nfilas;
                        botonX.Width = ancho / ncolumnas;
                        botonX.Tag = indiceLista;
                        botonX.Click += new EventHandler((sender,e)=>aniadir_cesta(sender,e,listaMedicamento));
                        botonX.MouseHover += new EventHandler((sender,e)=>mostrar_Informacion(sender,e,listaMedicamento));
                     

                    //cargar la imagen en el botón a través de un objeto MemoryStream
                    try
                    {
                        MemoryStream ms = new MemoryStream(listaMedicamento[indiceLista].Imagen);
                        botonX.BackgroundImage = System.Drawing.Image.FromStream(ms);
                        //necesito ajustar la imagen al tamaño del boton
                        botonX.BackgroundImageLayout = ImageLayout.Stretch;
                        //contenedor que ubica el botón el tlp
                        tlp.Controls.Add(botonX);
                        indiceLista++;
                    }
                    catch (Exception ex)
                    {

                    }
                   

                }
                
            }
        }

        //Metodo para mostrar la informacion al pasar el ratos por encima
        private void mostrar_Informacion(object sender, EventArgs e,List<claseMedicamento>lm)
        {
            Button boton = (Button)sender; //Indicamos que boton estamos seleccionando
            int indice = Convert.ToInt16(boton.Tag); //Cogemos el indicador
            lbNombre.Text = lm[indice].Nombre; //Pasamos el nombre del medicamento
            lbPrecio.Text = "" +Math.Round( lm[indice].Precio,2); //Pasamos el precio redondeado del medicamento 
            lbStockMin.Text =""+ lm[indice].Stockminimo; //Pasamos el stock minimo del medicamento
            lbStockActual.Text=""+ lm[indice].Stockactual; //Pasamos el stock actual del medicamento

        }

        //Metodo para añadir medicamentos a la cesta
        private void aniadir_cesta(object sender, EventArgs e,List<claseMedicamento>lm)
        {
            Button botonx = (Button)sender; //Indicamos que boton estamos seleccionando

            //Si el boton refrescar esta a false el boton del medicamento se pondra a false
            if (btnRefrescar.Enabled==false)
                {
                botonx.Enabled = false; 
               }
            int posicion = Convert.ToInt16(botonx.Tag); //Obtener la posicion mediante el tag del boton
           
            //Si el stock del medicamento es mayor que 0 añadiremos a la variable total el precio del medicamento
            //Si no lo es, se enviara un messageBox diciendo que no quedan
           if (lm[posicion].Stockactual > 0)
            {
                total += lm[posicion].Precio;
                //Removemos la ultima columna del datagridView1 
                dataGridView1.Rows.RemoveAt(dataGridView1.RowCount - 1);
                //Añadimos al dataGridView una columna con el nombre del medicamento y el precio
                dataGridView1.Rows.Add(lm[posicion].Nombre, String.Format("{0:0.00}", listaMedicamento[posicion].Precio));
                //Añadimos al dataGridView una columna con el total y el nuevo precio total
                dataGridView1.Rows.Add("Total:", Math.Round(total,2));
                //Añadimos a listaCesta el medicamento
                listaCesta.Add(lm[posicion]);
               
                lbSuma.Text =String.Format("{0:0.00}",total);
            }
           else
            {
                MessageBox.Show("No hay queda en el almacén " + listaMedicamento[posicion].Nombre);
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            tabControl1.Controls.Clear();
            pictureBox1.Image = null;
            listaMedicamento.Clear();
            listaMedicamento = cnx.listar();
            try
            {
                cargarTPV(listaMedicamento);
            }
            catch (Exception ex) { }
        }

      

        private void button3_Click(object sender, EventArgs e)
        {
            pdfTicket=imprimirTicket();
           
            Actualizar_Tabla();
            Insertar_Factura();
            mandar_mail(pdfTicket);
            limpiar();
            btnRefrescar.Enabled = true;
        }

        //Metodo para mandar emails a partir de un correo
        private void mandar_mail(String pdfTicket)
        {
            try
            {
                string email = "augustobrigaprofe@gmail.com"; //Guardamos el email en un string
                string password = txtPasswordEmpresa.Text; //Obtenemos la contraseña de un TextBox

                var loginInfo = new NetworkCredential(email, password); //Creamos unas nueva credencial con la informacion del email
                var msg = new MailMessage(); //Creamos un nuevo mensaje
                var smtpClient = new SmtpClient("smtp.gmail.com", 25); //Creamos un nuevo cliente con los datos del servicio a usar

                //Pasamos configuraciones del mensaje
                msg.From = new MailAddress(email);
                msg.To.Add(new MailAddress(txtCliente.Text));
                msg.Subject = "Factura Farmacia ";
                msg.Body = "Ticket De compra";
                //Adjuntamos un fichero
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(pdfTicket);
                msg.Attachments.Add(attachment);
              
                

                msg.IsBodyHtml = true;

                //Cargamos en el smtpClient las credenciales y enviamos el mensaje
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                
                smtpClient.Send(msg);
                smtpClient.Dispose();
            }catch(Exception ex)
            {

            }
           
        }

        //Llamamos al metodo conectarBD.insertarFacturas y le pasamos la lista de la compra con el dni del cliente y el total de la compra
        private void Insertar_Factura()
        {
            cnx.insertarFacturas(listaCesta, txtdni.Text, total);
        }

        //Metodo para actualizar 
        private void Actualizar_Tabla()
        {
            //Si el boton refrescar esta a falso llama al metodo actualizarTratamiento
            if (btnRefrescar.Enabled==false)
            {

                cnx.actualizarTratamiento(listaCesta,lbDni.Text,Convert.ToInt16(DateTime.Now.Month));
               
            }
            //Llamamos al metodo Lanzar_actualizacion para restar el stock
            cnx.Lanzar_actualizacion(listaCesta);
            //Limpiamos listaMedicamentos y volvemos a llenarla con lo nuevos datos
            listaMedicamento.Clear();
            listaMedicamento = cnx.listar();

        }

        //Imprime la factura
        private String imprimirTicket()
        {
            MessageBox.Show("Total a pagar " + Math.Round(total,2));

            //Crear Tabla iTextSharp  desde una tabla de datos (datagridView)
            PdfPTable pdfTable = new PdfPTable(dataGridView1.ColumnCount);

            //padding
            pdfTable.DefaultCell.Padding = 3;

            //ancho que va a ocupar la tabla en el pdf
            pdfTable.WidthPercentage = 80;

            //alineación
            pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;

            //borde de las tablas
            pdfTable.DefaultCell.BorderWidth = 1;
            //Añadir fila de cabecera
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
                pdfTable.AddCell(cell);
            }

            //añadir filas
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    try
                    {
                        pdfTable.AddCell(cell.Value.ToString());
                    }
                    catch { }
                }
            }
            pdfTable.AddCell(DateTime.Now.ToString("MM-dd-yy"));
            pdfTable.AddCell(lbNombre.Text);

            //Exportar a pdf (ruta por defect
            string folderPath = "C:\\ticket\\";

            //si no existe el directoria se crea
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string nombreTicket =DateTime.Now.ToString("MM-dd-yy_HH-mm-ss")+".pdf";
            folderPath += nombreTicket;
            using (FileStream stream = new FileStream(folderPath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A6, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                
                pdfDoc.Close();
                stream.Close();
            }
            Process pc = new Process();
            pc.StartInfo.FileName = folderPath;
            pc.Start();
            dataGridView1.Rows.Add("Total:", 0);
            return folderPath;
        }
    


        private void button2_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        //Metodo para limpiar
        public void limpiar()
        {
            //Limpiamos listaCesta y limpiamos el datagridView, ponemos el total a 0 y el textBox lbSuma a 0
            listaCesta.Clear();
            dataGridView1.Rows.Clear();
            lbSuma.Text = "0";
            total = 0;
            dataGridView1.Rows.Add("Total:", 0);
        }

        //Metodo que al hacer dobleclick en una columna del datagridview te pregunta si quieres borrar el producto
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int posicion = dataGridView1.CurrentRow.Index;
            if (listaCesta.Count > 0)
            {
                if (posicion != dataGridView1.RowCount - 1)
                {
                    DialogResult resultado = MessageBox.Show("¿Quieres eliminar producto de la cesta?", "TPVFARM", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultado == DialogResult.Yes)
                {       
                   
                   
                        double precioParcial = listaCesta[posicion].Precio;
                        total = total - precioParcial;
                        listaCesta.RemoveAt(posicion);
                        dataGridView1.Rows.RemoveAt(posicion);
                        lbSuma.Text = String.Format("{0:0.00}", total);
                        dataGridView1.Rows.RemoveAt(dataGridView1.RowCount - 1);
                        dataGridView1.Rows.Add("TOTAL: ", Math.Round(total, 2));
                    }

                }
            }

        }

        //Comprueba los usuarios
        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
               
                int nivel=cnx.buscarUsuario(txtdni.Text,txtPwd.Text);
                if (nivel == 0) 
                    MessageBox.Show("Usuario No existe");
                else
                {
                    if (nivel == 1)
                    {
                        MessageBox.Show("Nivel administrador");
                        btnGestion.Visible = true;
                        pictureBox2.Visible = true;
                        upAll();

                    }
                    else
                    {
                        MessageBox.Show("Usuario dependiente");
                        upAll();                      

                    }
                    lbUsuario.Text = txtdni.Text;
                    txtdni.Visible = false;
                    txtPwd.Visible = false;
                    btnCerrarSesion.Visible = true;
                    btnPagar.Visible = true;
                    btnVaciarCesta.Visible = true;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtdni.Text = "";
            txtPwd.Text = "";
            txtdni.Visible = true;
            txtPwd.Visible = true;
            lbUsuario.Text = "";
            downAll();
            txtRecoje.Text = "";
            txtRecoje.Enabled = true;
            txtRecoje.Visible = true;

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult salir = MessageBox.Show("¿Quieres salir?", "TPV Farmacia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (salir == DialogResult.Yes)
            {
                Close();
            }
        }

        private void btnGestion_Click(object sender, EventArgs e)
        {
         
            formGestion fg = new formGestion();
            fg.ShowDialog();
        }

       

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tabControl1.Controls.Clear();
            MedEncontrados.Clear();

            foreach (claseMedicamento med in listaMedicamento)
            {
                if (med.Nombre.StartsWith(textBox1.Text, true, null))
                {
                    MedEncontrados.Add(med);
                }
            }
            cargarTPV(MedEncontrados); //o bien recargar la lista principal con esta lista Aux
            textBox1.Focus();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           OpenFileDialog    OD = new OpenFileDialog();
            OD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (OD.ShowDialog() == DialogResult.OK)
                pictureBox1.Load(OD.FileName);
            BarcodeDecoder Scanner = new BarcodeDecoder();
            Result result = Scanner.Decode(new Bitmap(pictureBox1.Image));
      
            txtCodigoBarra.Text = result.Text;
        }

        private void txtPasswordEmpresa_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            txtPasswordEmpresa.Enabled = false;
        }

    

        


        private void txtCodigoBarra_TextChanged(object sender, EventArgs e)
        {
            tabControl1.Controls.Clear();
            MedEncontrados.Clear();

            foreach (claseMedicamento med in listaMedicamento)
            {
                if (med.Indice == Convert.ToInt16(txtCodigoBarra.Text))
                {
                    MedEncontrados.Add(med);
                }
            }
            cargarTPV(MedEncontrados);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog OD = new OpenFileDialog();
            OD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (OD.ShowDialog() == DialogResult.OK)
                pictureBox2.Load(OD.FileName);
            BarcodeDecoder Scanner = new BarcodeDecoder();
            Result result = Scanner.Decode(new Bitmap(pictureBox2.Image));

            lbDni.Text = result.Text;
            
            listaTarjetas = cnx.listarTarjetas();
            btnRefrescar.Enabled = false;
            for (int i = 0; i < listaTarjetas.Count; i++)
            {
                if (listaTarjetas[i].Dni == lbDni.Text)
                {
                    upAll();
                    int mes =Convert.ToInt16( DateTime.Now.Month);
                    lbMail.Text = listaTarjetas[i].Email;
                    txtCliente.Text= listaTarjetas[i].Email;
                    listaTratamientos.Clear();
                    listaTratamientos = cnx.listarTratamientos(listaTarjetas[i].Dni,mes);
                    tabControl1.Controls.Clear();
                    MedEncontrados.Clear();
                    
                    for (int j = 0; j < listaTratamientos.Count; j++)
                    {

                        foreach (claseMedicamento med in listaMedicamento)
                        {
                            if (med.Nombre==listaTratamientos[j].Medicamento)
                            {
                                MedEncontrados.Add(med);
                            }
                        }
                        
                    }
                    cargarTPV(MedEncontrados);
                }
            }
            
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtLector_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void txtLector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lbDni.Text = txtLector.Text;
                listaTarjetas = cnx.listarTarjetas();
                btnRefrescar.Enabled = false;
                for (int i = 0; i < listaTarjetas.Count; i++)
                {
                    if (listaTarjetas[i].Dni == lbDni.Text)
                    {
                        int mes = Convert.ToInt16(DateTime.Now.Month);
                        lbMail.Text = listaTarjetas[i].Email;
                        txtCliente.Text = listaTarjetas[i].Email;
                        listaTratamientos.Clear();
                        listaTratamientos = cnx.listarTratamientos(listaTarjetas[i].Dni, mes);
                        tabControl1.Controls.Clear();
                        MedEncontrados.Clear();
                        for (int j = 0; j < listaTratamientos.Count; j++)
                        {



                            foreach (claseMedicamento med in listaMedicamento)
                            {
                                if (med.Nombre == listaTratamientos[j].Medicamento)
                                {
                                    MedEncontrados.Add(med);
                                }
                            }

                        }
                        cargarTPV(MedEncontrados);
                    }
                }
            }
        }

        private void txtLaser2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

            }
        }

        private void s(object sender, EventArgs e)
        {

        }

        private void txtPasswordEmpresa_TextChanged(object sender, EventArgs e)
        {

        }

        //Metodo para desbloquear todo
        private void upAll() {

            dataGridView1.Enabled = true;
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;
            btnRefrescar.Enabled = true;
            txtPasswordEmpresa.Enabled = true;
            txtCliente.Enabled = true;
            tabControl1.Enabled = true;
            textBox1.Enabled = true;
            pictureBox1.Enabled = true;
            txtLaser2.Enabled = true;
            txtdni.Enabled = true;
            btnCerrarSesion.Enabled = true;
            btnSalir.Enabled = true;
            txtLector.Enabled = true;
            btnGestion.Enabled = true;
            btnPagar.Enabled = true;
            btnVaciarCesta.Enabled = true;
            txtPwd.Enabled = true;
            txtCodigoBarra.Enabled = true;

            txtPwd.Visible = true;
            txtCodigoBarra.Visible = true;
            dataGridView1.Visible = true;
            numericUpDown1.Visible = true;
            numericUpDown2.Visible = true;
            btnRefrescar.Visible = true;
            txtPasswordEmpresa.Visible = true;
            txtCliente.Visible = true;
            tabControl1.Visible = true;
            textBox1.Visible = true;
            pictureBox1.Visible = true;
            txtLaser2.Visible = true;
            txtdni.Visible = true;

            btnSalir.Visible = true;
            txtLector.Visible = true;


        }
        //Metodo para bloquear todo
        private void downAll() {

            dataGridView1.Enabled = false;
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            btnRefrescar.Enabled = false;
            txtPasswordEmpresa.Enabled = false;
            txtCliente.Enabled = false;
            txtdni.Enabled = false;
            tabControl1.Enabled = false;
            textBox1.Enabled = false;
            pictureBox1.Enabled = false;
            txtPwd.Enabled = false;
            txtLaser2.Enabled = false;
            btnCerrarSesion.Enabled = false;
            btnSalir.Enabled = false;
            txtLector.Enabled = false;
            btnGestion.Enabled = false;
            btnPagar.Enabled = false;
            btnVaciarCesta.Enabled = false;
            pictureBox2.Enabled = false;
            txtCodigoBarra.Enabled = false;
            pictureBox3.Enabled = true;

            txtdni.Visible = false;
            pictureBox3.Visible = true;
            txtPwd.Visible = false;
            txtCodigoBarra.Visible = false;
            dataGridView1.Visible = false;
            numericUpDown1.Visible = false;
            numericUpDown2.Visible = false;
            btnRefrescar.Visible = false;
            txtPasswordEmpresa.Visible = false;
            txtCliente.Visible = false;
            tabControl1.Visible = false;
            textBox1.Visible = false;
            pictureBox1.Visible = false;
            txtLaser2.Visible = false;
            btnCerrarSesion.Visible = false;
            btnSalir.Visible = false;
            txtLector.Visible = false;
            btnGestion.Visible = false;
            btnPagar.Visible = false;
            btnVaciarCesta.Visible = false;
            pictureBox2.Visible = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            OpenFileDialog OD = new OpenFileDialog();
            OD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (OD.ShowDialog() == DialogResult.OK)
                pictureBox3.Load(OD.FileName);
            BarcodeDecoder Scanner = new BarcodeDecoder();
            Result result = Scanner.Decode(new Bitmap(pictureBox3.Image));

            txtRecoje.Text = result.Text;

            if (txtRecoje.Text.Equals("3333"))
                upAll();
            else if (txtRecoje.Text.Equals("33943444"))
                upAll();
            else if (txtRecoje.Text.Equals("4444"))
                upAll();
            else {
                MessageBox.Show("Usuario no reconocido");
            }

            txtRecoje.Enabled = false;
            txtRecoje.Visible = false;
            pictureBox3.Enabled = false;
            pictureBox3.Visible = false;
        }

        private void txtRecoje_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtRecoje.Text.Equals("3333")) {

                    upAll();
                    txtRecoje.Enabled = false;
                    txtRecoje.Visible = false;
                    pictureBox3.Enabled = false;
                    pictureBox3.Visible = false;
                }
                    
                else if (txtRecoje.Text.Equals("33943444"))
                {
                    upAll();
                    txtRecoje.Enabled = false;
                    txtRecoje.Visible = false;
                    pictureBox3.Enabled = false;
                    pictureBox3.Visible = false;
                }
                else if (txtRecoje.Text.Equals("4444"))
                {
                    upAll();
                    txtRecoje.Enabled = false;
                    txtRecoje.Visible = false;
                    pictureBox3.Enabled = false;
                    pictureBox3.Visible = false;
                }
                else
                {
                    MessageBox.Show("Usuario no reconocido");
                }
            }
            
        }
    }
}
