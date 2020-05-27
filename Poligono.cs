/**
  Autor: Dalton Solano dos Reis
**/

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System.Collections.Generic;

// ATENÇÃO: remover: "Privado_"
namespace gcgcg
{
    internal class Poligono : ObjetoGeometria
    {
        public Poligono(string rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
        }

        private bool aberto = false;

        public bool getAberto()
        {
            return this.aberto;
        }

        public void setAberto(bool status)
        {
            this.aberto = status;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(base.PrimitivaTipo);
            foreach (Ponto4D pto in pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.End();
        }

        public void desenharAbeto()
        {
            GL.Color3(OpenTK.Color.White);
            GL.Begin(BeginMode.Lines);
            foreach (Ponto4D pto in pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.End();

        }

        public List<Ponto4D> getPontosPoligono()
        {
            return base.pontosLista;
        }

        public Ponto4D getVertice(Ponto4D ponto)
        {
            foreach (Ponto4D pontoPoligono in base.pontosLista)
            {
                if (pontoPoligono.X == ponto.X && pontoPoligono.Y == ponto.Y)
                {
                    return pontoPoligono;
                }
            }
            return null;
        }
        //TODO: melhorar para exibir não só a lsita de pontos (geometria), mas também a topologia ... poderia ser listado estilo OBJ da Wavefrom
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Poligono: " + base.rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
            }
            return (retorno);
        }

    }
}