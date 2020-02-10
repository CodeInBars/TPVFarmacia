using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tpvFarmacia
{
    class claseTarjetaSanitaria
    {
        String dni; //Dni de la persona
        String nombre; //Nombre de la persona
        String email; //Email de la persona
        DateTime fechaNacimiento; //Fecha de nacimiento de la persona

        //Metodos
        public string Dni
        {
            get
            {
                return dni;
            }

            set
            {
                dni = value;
            }
        }

        public string Nombre
        {
            get
            {
                return nombre;
            }

            set
            {
                nombre = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public DateTime FechaNacimiento
        {
            get
            {
                return fechaNacimiento;
            }

            set
            {
                fechaNacimiento = value;
            }
        }

        //Constructores
        public claseTarjetaSanitaria(string dni, string nombre, string email, DateTime fechaNacimiento)
        {
            this.Dni = dni;
            this.Nombre = nombre;
            this.Email = email;
            this.FechaNacimiento = fechaNacimiento;
        }
        public claseTarjetaSanitaria()
        {

        }

    }
}
