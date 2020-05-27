using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
    class SegReta : ObjetoGeometria
    {
        private Color color;
        private int lineWidth;
        public SegReta(string rotulo, Objeto paiRef, Ponto4D[] pontos, int lineWidth, Color color) : base(rotulo, paiRef)
        {
            this.color = color;
            this.lineWidth = lineWidth;

            foreach (var ponto in pontos)
            {
                base.PontosAdicionar(ponto);
            }
        }

        protected override void DesenharObjeto()
        {
            GL.LineWidth(this.lineWidth);
            GL.Color3(this.color);
            GL.Begin(BeginMode.Lines);
            foreach (Ponto4D pto in pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.End();
        }
        //TODO: melhorar para exibir não só a lsita de pontos (geometria), mas também a topologia ... poderia ser listado estilo OBJ da Wavefrom
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Retangulo: " + base.rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
            }
            return (retorno);
        }
        public void drawTriangle()
        {
            GL.Color3(this.color);
            GL.LineWidth(this.lineWidth);
            //P1 / P2
            Ponto4D[] pontos = base.pontosLista.ToArray();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(pontos[0].X, pontos[0].Y);
            GL.Vertex2(pontos[1].X, pontos[1].Y);
            GL.End();
            //P2 / P3
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(pontos[1].X, pontos[1].Y);
            GL.Vertex2(pontos[2].X, pontos[2].Y);
            GL.End();
            //P3 / P1
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(pontos[2].X, pontos[2].Y);
            GL.Vertex2(pontos[0].X, pontos[0].Y);
            GL.End();
        }

        public static void dynamicLine(Ponto4D ponto4DBase, Color color, int size, int rotationGrados)
        {
            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(ponto4DBase.X, ponto4DBase.Y);

            double degInRad = rotationGrados * 3.1416 / 180;
            GL.Vertex2(Math.Cos(degInRad) * size + ponto4DBase.X, Math.Sin(degInRad) * size + ponto4DBase.Y);
            GL.End();

        }

    }
}