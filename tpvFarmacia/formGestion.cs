using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using MessagingToolkit.Barcode;

namespace tpvFarmacia
{
    public partial class formGestion : Form
    {
        conectarBD cnx;
        List<claseMedicamento> listaMedicamento = new List<claseMedicamento>();
        List<claseMedicamento> listaMedicamento2 = new List<claseMedicamento>();
        String nombreImagen;
        claseMedicamento med = new claseMedicamento();
        String pdfTicket;

        public formGestion()
        {
            InitializeComponent();
        }

        private void formGestion_Load(object sender, EventArgs e)
        {
            cnx = new conectarBD();
            listaMedicamento = cnx.listar();
            dataGridView1.DataSource = listaMedicamento;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fs = new FileStream(nombreImagen, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bloque = br.ReadBytes((int)fs.Length);
                cnx.Insertar(txtNombre.Text, Convert.ToDouble(txtPrecio.Text), bloque, Convert.ToInt16(txtStockMin.Text), Convert.ToInt16(txtStockActual.Text));
            }
            catch (Exception)
            {
                MessageBox.Show("datos incompletos");
            }

         }

        //Cargar codigo de un nuevo medicamento
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            claseMedicamento cm = new claseMedicamento();
            OpenFileDialog OD = new OpenFileDialog();
            OD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (OD.ShowDialog() == DialogResult.OK)
                pictureBox1.Load(OD.FileName);
            BarcodeDecoder Scanner = new BarcodeDecoder();
            Result result = Scanner.Decode(new Bitmap(pictureBox1.Image));

            lblScannerNumero.Text = result.Text;
            cm = cnx.buscarmedicamento(lblScannerNumero.Text);
            txtNombre.Text = cm.Nombre;
            txtPrecio.Text = Convert.ToString(cm.Precio);
            txtStockMin.Text = Convert.ToString(cm.Stockminimo);
            txtStockActual.Text = Convert.ToString(cm.Stockactual);

            cnx.añadirMedicamento(cm);

        }

        private void txt_DoubleClick(object sender, EventArgs e)
        {
        
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
        }
        //Configuracion dataGridView1
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            txtNombreMod.Text = listaMedicamento[dataGridView1.CurrentRow.Index].Nombre;
            txtPrecioMod.Text = Convert.ToString(listaMedicamento[dataGridView1.CurrentRow.Index].Precio);
            txtStockActMod.Text = Convert.ToString(listaMedicamento[dataGridView1.CurrentRow.Index].Stockactual);
            txtStockMinMod.Text = Convert.ToString(listaMedicamento[dataGridView1.CurrentRow.Index].Stockminimo);
            MemoryStream ms = new MemoryStream(listaMedicamento[dataGridView1.CurrentRow.Index].Imagen);
            pictureBoxMod.Image = System.Drawing.Image.FromStream(ms);
            med.Imagen = listaMedicamento[dataGridView1.CurrentRow.Index].Imagen;
            lbIndice.Text = Convert.ToString(listaMedicamento[dataGridView1.CurrentRow.Index].Indice);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            med.Indice = Convert.ToInt16(lbIndice.Text);
            med.Nombre = txtNombreMod.Text;
            med.Precio = Convert.ToDouble(txtPrecioMod.Text);
            med.Stockminimo = Convert.ToInt16(txtStockMinMod.Text);
            med.Stockactual = Convert.ToInt16(txtStockActMod.Text);
            cnx.modificarMedicamento(med);
            listaMedicamento.Clear();
            listaMedicamento = cnx.listar();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = listaMedicamento;
        }

        private void pictureBoxMod_Click(object sender, EventArgs e)
        {
            String imagen;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    imagen = openFileDialog1.FileName;
                    pictureBoxMod.Image = System.Drawing.Image.FromFile(imagen);
                    FileStream fs = new FileStream(imagen, FileMode.Open, FileAccess.Read);
                    long tamanio = fs.Length;

                    BinaryReader br = new BinaryReader(fs);
                    byte[] bloque = br.ReadBytes((int)fs.Length);
                    fs.Read(bloque, 0, Convert.ToInt32(tamanio));

                    med.Imagen = bloque;
                    // MemoryStream ms = new MemoryStream(bloque);

                    //  listadoMedicamento[dataGridView1.CurrentRow.Index].Imagen = bloque;


                    // cargarBotones();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("El archivo seleccionado no es un tipo de imagen");
            }
        }

        private void formGestion_Leave(object sender, EventArgs e)
        {
            
        }

        private void btnGenerarPedido_Click(object sender, EventArgs e)
        {
            listaMedicamento2 = cnx.listarMinimo();
            imprimirTicket(listaMedicamento2);
            try
            {
                string email = txtEmail.Text; //Guardamos el email en un string
                string password = txtPassword.Text; //Obtenemos la contraseña de un TextBox

                var loginInfo = new NetworkCredential(email, password); //Creamos unas nueva credencial con la informacion del email
                var msg = new MailMessage(); //Creamos un nuevo mensaje
                var smtpClient = new SmtpClient("smtp.gmail.com", 25); //Creamos un nuevo cliente con los datos del servicio a usar

                //Pasamos configuraciones del mensaje
                msg.From = new MailAddress(email);
                msg.To.Add(new MailAddress("profeaugustobriga@gmail.com"));
                msg.Subject = "Stock por debajo Ismael";
                msg.Body = "Stock por debajo";

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
            }
            catch (Exception ex)
            {

            }
            listaMedicamento2.Clear();
        }

        //Imprime la factura
        private String imprimirTicket(List<claseMedicamento> lm)
        {

            
            
            for (int i = 0; i < lm.Count; i++) {
                dataGridView2.Rows.Add(lm[i].Nombre, lm[i].Stockminimo, lm[i].Stockactual, (lm[i].Stockminimo - lm[i].Stockactual));
            }
            
            //Crear Tabla iTextSharp  desde una tabla de datos (datagridView)
            PdfPTable pdfTable = new PdfPTable(dataGridView2.ColumnCount);

            //padding
            pdfTable.DefaultCell.Padding = 3;

            //ancho que va a ocupar la tabla en el pdf
            pdfTable.WidthPercentage = 80;

            //alineación
            pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;

            //borde de las tablas
            pdfTable.DefaultCell.BorderWidth = 1;
            //Añadir fila de cabecera
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
                pdfTable.AddCell(cell);
            }

            //añadir filas
            foreach (DataGridViewRow row in dataGridView2.Rows)
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
            

            //Exportar a pdf (ruta por defect
            string folderPath = "C:\\ticket\\";

            //si no existe el directoria se crea
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string nombreTicket = DateTime.Now.ToString("MM-dd-yy_HH-mm-ss") + "stock.pdf";
            folderPath += nombreTicket;
            pdfTicket = folderPath;
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
            return folderPath;
        }

        //Añadir medicamento a partir del codigo scaneado
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                claseMedicamento cm = new claseMedicamento();
                cm = cnx.buscarmedicamento(textBox1.Text);
                txtNombre.Text = cm.Nombre;
                txtPrecio.Text = Convert.ToString(cm.Precio);
                txtStockMin.Text = Convert.ToString(cm.Stockminimo);
                txtStockActual.Text = Convert.ToString(cm.Stockactual);

                cnx.añadirMedicamento(cm);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            cnx.exbackup();
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                String nombreFichero = "C:\\backup\\backup.sql";
                //Si Creamos un nuevo objeto se inicia una nueva conexion
                MySqlConnection conexion = new MySqlConnection();
                conexion.ConnectionString = "Server=remotemysql.com;Database=RAlXBB5k2e;Uid=RAlXBB5k2e;pwd=W6vqMUnP4r";
                conexion.Open();
                MySqlCommand comando = new MySqlCommand();
                MySqlBackup mb = new MySqlBackup(comando);

                comando.Connection = conexion;
                mb.ImportFromFile(nombreFichero);
                conexion.Close();
                MessageBox.Show("Se ha importado la base de datos");
            }
            catch (Exception) {
                MessageBox.Show("Error al importar la base de datos");
            }
            
        }
    }
}
