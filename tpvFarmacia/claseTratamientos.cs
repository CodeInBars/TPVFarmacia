using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tpvFarmacia
{
    class claseTratamientos
    {
        int identificador; //Identifica el tratamiento
        String dni; //Dni de la persona a la que pertenece el tratamiento
        String medicamento; //Nombre del Medicamento de la persona
        int mes; //Cuantos meses lo recoje
        int recogido; //Indica si esta recojido o no

        //Metodos
        public int Identificador
        {
            get
            {
                return identificador;
            }

            set
            {
                identificador = value;
            }
        }

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

        public string Medicamento
        {
            get
            {
                return medicamento;
            }

            set
            {
                medicamento = value;
            }
        }

        public int Mes
        {
            get
            {
                return mes;
            }

            set
            {
                mes = value;
            }
        }

        public int Recogido
        {
            get
            {
                return recogido;
            }

            set
            {
                recogido = value;
            }
        }

        //Constructores
        public claseTratamientos(int identificador, string dni, string medicamento, int mes, int recogido)
        {
            this.Identificador = identificador;
            this.Dni = dni;
            this.Medicamento = medicamento;
            this.Mes = mes;
            this.Recogido = recogido;
        }

        public claseTratamientos()
        {
        }
    }
}
