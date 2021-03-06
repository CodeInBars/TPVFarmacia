﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tpvFarmacia
{
    class claseMedicamento
    {
        int indice; //Indice del medicamento
        String nombre; //Nombre del medicamento
        double precio; //Precio del medicamento
        byte[] imagen; //Imagen del medicamento
        int stockminimo; //Stock Minimo del medicamento
        int stockactual; //Stock Actual del medicamento

        //Metodos
        public int Indice
        {
            get
            {
                return indice;
            }

            set
            {
                indice = value;
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

        public double Precio
        {
            get
            {
                return precio;
            }

            set
            {
                precio = value;
            }
        }

        public byte[] Imagen
        {
            get
            {
                return imagen;
            }

            set
            {
                imagen = value;
            }
        }

        public int Stockminimo
        {
            get
            {
                return stockminimo;
            }

            set
            {
                stockminimo = value;
            }
        }

        public int Stockactual
        {
            get
            {
                return stockactual;
            }

            set
            {
                stockactual = value;
            }
        }

        //Constructores
        public claseMedicamento(int indice, string nombre, float precio, byte[] imagen, int stockminimo, int stockactual)
        {
            this.Indice = indice;
            this.Nombre = nombre;
            this.Precio = precio;
            this.Imagen = imagen;
            this.Stockminimo = stockminimo;
            this.Stockactual = stockactual;
        }
        public claseMedicamento()
        {

        }
    }
}
