/**
  Autor: Dalton Solano dos Reis
**/

// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo = null;

        private Mundo(int width, int height) : base(width, height) { }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        private CameraOrtho camera = new CameraOrtho();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;
        private bool bBoxDesenhar = false;
        int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
        private Poligono objetoNovo = null;
        private String objetoId = "A";
        private Ponto4D verticeMaisProximo;
        private int xPointClick;
        private int yPointClick;

        private bool TRE = false;
        private bool scale = false;

        private bool rotatate = false;

        private bool createDynamicPolygon = false;
        private bool poligonoQualquer = false;

        private int translacaoX = 0;
        private int translacaoY = 0;

        private int lastPositionX;
        private int lastPositionY;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = 0; camera.xmax = 600; camera.ymin = 0; camera.ymax = 600;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            GL.ClearColor(OpenTK.Color.Gray);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Sru3D();

            foreach (Object objeto in this.objetosLista)
            {
                Poligono poligono = (Poligono)objeto;
                if (!poligono.getAberto())
                {
                    poligono.Desenhar();
                }
                else
                {
                    poligono.desenharAbeto();
                }
            }

            if (this.objetoSelecionado != null)
            {
                objetoSelecionado.BBox.Desenhar();
            }

            this.SwapBuffers();
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
                Utilitario.AjudaTeclado();
            else if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
            }
            else if (e.Key == Key.O)
                bBoxDesenhar = !bBoxDesenhar;
            else if (e.Key == Key.Enter)
            {
                if (objetoNovo != null)
                {
                    objetoNovo.PontosRemoverUltimo();   // N3-Exe6: "truque" para deixar o rastro
                    objetoSelecionado = objetoNovo;
                    objetoNovo = null;
                }
            }
            else if (e.Key == Key.Space)
            {
                if (objetoNovo == null)
                {
                    objetoNovo = new Poligono(objetoId + 1, null);
                    objetosLista.Add(objetoNovo);
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));  // N3-Exe6: "troque" para deixar o rastro
                }
                else
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
            }
            else if (e.Key == Key.R)
            {
                if (!poligonoQualquer)
                {
                    this.poligonoQualquer = true;
                    objetoNovo = new Poligono(objetoId + 1, null);
                    objetoNovo.setAberto(true);
                    objetoNovo.PontosAdicionar(new Ponto4D(this.mouseX, this.mouseY));
                    objetoNovo.PontosAdicionar(new Ponto4D(this.mouseX, this.mouseY));
                    this.objetosLista.Add(objetoNovo);
                    objetoSelecionado = objetoNovo;
                }
                else
                {
                    this.poligonoQualquer = false;
                    objetoSelecionado = null;
                    objetoNovo.PontosRemoverUltimo();
                    objetoNovo = null;
                }
            }
            else if (e.Key == Key.L)
            {
                //Pegar o último vertice e editar
                if (this.objetoSelecionado != null && this.verticeMaisProximo == null)
                {
                    Poligono poligonoSelecionado = (Poligono)this.objetoSelecionado;
                    List<Ponto4D> ponto4DsPoligono = poligonoSelecionado.getPontosPoligono();
                    int diferencaXSelecionado = 0;
                    int diferencaYSelecionado = 0;
                    foreach (Ponto4D pontoPoligono in ponto4DsPoligono)
                    {
                        int diferencaXCalculado = Math.Abs(Math.Abs(this.mouseX) - Math.Abs((int)pontoPoligono.X));
                        int diferencaYCalculado = Math.Abs(Math.Abs(this.mouseY) - Math.Abs((int)pontoPoligono.Y));
                        if (this.verticeMaisProximo == null || (diferencaXCalculado < diferencaXSelecionado && diferencaYCalculado < diferencaYSelecionado))
                        {
                            this.verticeMaisProximo = pontoPoligono;
                            diferencaXSelecionado = diferencaXCalculado;
                            diferencaYSelecionado = diferencaYCalculado;
                        }
                    }

                }
                else
                {
                    this.objetoSelecionado = null;
                    this.verticeMaisProximo = null;
                }
            }
            else if (e.Key == Key.T)
            {
                //Translação
                Poligono poligono = (Poligono)this.objetoSelecionado;
                Transformacao4D transformacao4D = poligono.Matriz;
                double[] teste = transformacao4D.ObterDados();
                this.translacaoX++;
                this.translacaoY++;
                teste[11] = this.translacaoX;
                teste[12] = this.translacaoY;
                teste[13] = this.translacaoY;
                Console.Write("");
            }
            else if (e.Key == Key.Q)
            {
                //Encerrar
                objetoNovo.PontosRemoverUltimo();
                this.createDynamicPolygon = false;
                objetoNovo = null;
                objetoSelecionado = null;
            }
            else if (e.Key == Key.N)
            {
                if (!this.createDynamicPolygon)
                {
                    this.createDynamicPolygon = true;
                    objetoSelecionado = null;
                    objetoNovo = new Poligono(objetoId + 1, null);
                    this.objetosLista.Add(objetoNovo);
                    objetoNovo.PontosAdicionar(new Ponto4D(this.mouseX, this.mouseY));
                    objetoNovo.PontosAdicionar(new Ponto4D(this.mouseX, this.mouseY));
                    objetoSelecionado = objetoNovo;
                }
                else
                {
                    objetoNovo.PontosUltimo().X = mouseX;
                    objetoNovo.PontosUltimo().Y = mouseY;
                    objetoNovo.PontosAdicionar(new Ponto4D(this.mouseX, this.mouseY));
                }

            }
            else if (objetoSelecionado != null)
            {
                if (e.Key == Key.M)
                    Console.WriteLine(objetoSelecionado.Matriz);
                else if (e.Key == Key.P)
                    Console.WriteLine(objetoSelecionado);
                else if (e.Key == Key.I)
                    objetoSelecionado.AtribuirIdentidade();
                //TODO: não está atualizando a BBox com as transformações geométricas
                else if (e.Key == Key.Left)
                    objetoSelecionado.TranslacaoXYZ(-10, 0, 0);
                else if (e.Key == Key.Right)
                    objetoSelecionado.TranslacaoXYZ(10, 0, 0);
                else if (e.Key == Key.Up)
                    objetoSelecionado.TranslacaoXYZ(0, 10, 0);
                else if (e.Key == Key.Down)
                    objetoSelecionado.TranslacaoXYZ(0, -10, 0);
                else if (e.Key == Key.PageUp)
                    objetoSelecionado.EscalaXYZ(2, 2, 2);
                else if (e.Key == Key.PageDown)
                    objetoSelecionado.EscalaXYZ(0.5, 0.5, 0.5);
                else if (e.Key == Key.Home)
                    objetoSelecionado.EscalaXYZBBox(0.5, 0.5, 0.5);
                else if (e.Key == Key.B)
                    if (!TRE)
                    {
                        TRE = true;
                    }
                    else
                    {
                        TRE = false;
                    }
                else if (e.Key == Key.C)
                    if (!scale)
                    {
                        scale = true;
                    }
                    else
                    {
                        scale = false;
                    }
                else if (e.Key == Key.X)
                    if (!this.rotatate)
                    {
                        this.rotatate = true;
                    }
                    else
                    {
                        this.rotatate = false;
                    }

                else if (e.Key == Key.End)
                    objetoSelecionado.EscalaXYZBBox(2, 2, 2);
                else if (e.Key == Key.Number1)
                    objetoSelecionado.Rotacao(10);
                else if (e.Key == Key.Number2)
                    objetoSelecionado.Rotacao(-10);
                else if (e.Key == Key.Number3)
                    objetoSelecionado.RotacaoZBBox(10);
                else if (e.Key == Key.Number4)
                    objetoSelecionado.RotacaoZBBox(-10);
                else if (e.Key == Key.Number9)
                    objetoSelecionado = null;
                else if (e.Key == Key.Number5)
                {
                    Poligono poligonoSelecionado = (Poligono)objetoSelecionado;
                    poligonoSelecionado.PrimitivaCor = OpenTK.Color.Red;
                }
                else if (e.Key == Key.Number6)
                {
                    Poligono poligonoSelecionado = (Poligono)objetoSelecionado;
                    poligonoSelecionado.PrimitivaCor = OpenTK.Color.Green;
                }
                else if (e.Key == Key.Number7)
                {
                    Poligono poligonoSelecionado = (Poligono)objetoSelecionado;
                    poligonoSelecionado.PrimitivaCor = OpenTK.Color.Blue;
                }
                else
                    Console.WriteLine(" __ Tecla não implementada.");
            }
            else
                Console.WriteLine(" __ Tecla não implementada.");
        }

        //TODO: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y

            if (this.lastPositionX == 0 && this.lastPositionY == 0)
            {
                this.lastPositionX = mouseX;
                this.lastPositionY = mouseY;
            }

            if ((objetoNovo != null))
            {
                objetoNovo.PontosUltimo().X = mouseX;
                objetoNovo.PontosUltimo().Y = mouseY;
            }
            else if (objetoSelecionado != null && this.verticeMaisProximo != null)
            {
                Poligono poligonoSelecionado = (Poligono)objetoSelecionado;
                Ponto4D verticeSelecionado = poligonoSelecionado.getVertice(verticeMaisProximo);
                verticeSelecionado.X = mouseX;
                verticeSelecionado.Y = mouseY;
            }
            else if (objetoSelecionado != null && this.TRE)
            {
                if (mouseX < this.lastPositionX && mouseY < this.lastPositionY)
                {
                    objetoSelecionado.TranslacaoXYZ(-2, -2, 0);
                    this.lastPositionX = mouseX;
                    this.lastPositionY = mouseY;
                }
                else if (mouseX > this.lastPositionX && mouseY > this.lastPositionY)
                {
                    objetoSelecionado.TranslacaoXYZ(2, 2, 0);
                    this.lastPositionX = mouseX;
                    this.lastPositionY = mouseY;
                }
                else if (mouseX < this.lastPositionX && mouseY > this.lastPositionY)
                {
                    objetoSelecionado.TranslacaoXYZ(-2, 2, 0);
                    this.lastPositionX = mouseX;
                    this.lastPositionY = mouseY;
                }
                else if (mouseX > this.lastPositionX && mouseY < this.lastPositionY)
                {
                    objetoSelecionado.TranslacaoXYZ(2, -2, 0);
                    this.lastPositionX = mouseX;
                    this.lastPositionY = mouseY;
                }
                else if (mouseX < this.lastPositionX)
                {
                    objetoSelecionado.TranslacaoXYZ(-2, 0, 0);
                    this.lastPositionX = mouseX;
                }
                else if (mouseX > this.lastPositionX)
                {
                    objetoSelecionado.TranslacaoXYZ(2, 0, 0);
                    this.lastPositionX = mouseX;
                }
                else if (mouseY < this.lastPositionY)
                {
                    objetoSelecionado.TranslacaoXYZ(0, -2, 0);
                    this.lastPositionY = mouseY;
                }
                else if (mouseY > this.lastPositionY)
                {
                    objetoSelecionado.TranslacaoXYZ(0, 2, 0);
                    this.lastPositionY = mouseY;
                }
            }
            else if (objetoSelecionado != null && this.scale)
            {
                if (mouseX < this.lastPositionX)
                {
                    objetoSelecionado.EscalaXYZBBox(1.05, 1.05, 1.05);
                    this.lastPositionX = mouseX;
                }
                else if (mouseX > this.lastPositionX)
                {
                    objetoSelecionado.EscalaXYZBBox(0.95, 0.95, 0.95);
                    this.lastPositionX = mouseX;
                }
                else if (mouseY < this.lastPositionY)
                {
                    objetoSelecionado.EscalaXYZBBox(0.95, 0.95, 0.95);
                    this.lastPositionY = mouseY;
                }
                else if (mouseY > this.lastPositionY)
                {
                    objetoSelecionado.EscalaXYZBBox(1.05, 1.05, 1.05);
                    this.lastPositionY = mouseY;
                }
            }

            else if (objetoSelecionado != null && this.rotatate)
            {
                if (mouseX < this.lastPositionX)
                {
                    objetoSelecionado.RotacaoZBBox(2);
                    this.lastPositionX = mouseX;
                }
                else if (mouseX > this.lastPositionX)
                {
                    objetoSelecionado.RotacaoZBBox(-2);
                    this.lastPositionX = mouseX;
                }
                else if (mouseY < this.lastPositionY)
                {
                    objetoSelecionado.RotacaoZBBox(2);
                    this.lastPositionY = mouseY;
                }
                else if (mouseY > this.lastPositionY)
                {
                    objetoSelecionado.RotacaoZBBox(-2);
                    this.lastPositionY = mouseY;
                }
            }
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.xPointClick = e.X >= 300 ? (e.X - 300) + 300 : 300 - (300 - e.X);
            this.yPointClick = e.Y >= 300 ? 300 - (e.Y - 300) : 300 + (300 - e.Y);
            this.createDynamicPolygon = false;
            if (!this.poligonoQualquer)
            {
                objetoNovo = null;
            }

            if (e.Mouse.LeftButton.Equals(ButtonState.Pressed))
            {
                if (!this.poligonoQualquer)
                {
                    this.objetoSelecionado = null;
                    Poligono poligono;
                    BBox bBox;
                    List<Ponto4D> pontos;
                    Ponto4D pontoAtual, pontoAux;
                    double ti, xi;
                    int iteracoes = 0;
                    foreach (Object objeto in this.objetosLista)
                    {
                        poligono = (Poligono)objeto;
                        bBox = poligono.BBox;
                        pontos = new List<Ponto4D>();
                        if (xPointClick <= bBox.obterMaiorX && yPointClick <= bBox.obterMaiorY && xPointClick >= bBox.obterMenorX && yPointClick >= bBox.obterMenorY)
                        {
                            // Famosa ScanLine
                            pontos = poligono.getPontosPoligono();
                            for (int i = 0; i < pontos.Count; i++)
                            {
                                pontoAtual = pontos[i];
                                if ((i+1) == pontos.Count) 
                                {
                                    pontoAux = pontos[0];
                                } 
                                else 
                                {
                                    pontoAux = pontos[i+1];
                                }
                                ti = (yPointClick - pontoAtual.Y) / (pontoAux.Y - pontoAtual.Y);
                                if (ti > 0) 
                                {
                                    xi = (pontoAtual.X + (pontoAux.X - pontoAtual.X)*ti);
                                    if (xi > xPointClick) 
                                    {
                                        iteracoes++;
                                    }
                                }
                            }

                            if (iteracoes % 2 != 0)
                            {
                                this.objetoSelecionado = poligono;
                            }
                        }
                    }
                }
                else
                {
                    objetoNovo.PontosUltimo().X = mouseX;
                    objetoNovo.PontosUltimo().Y = mouseY;
                    objetoNovo.PontosAdicionar(new Ponto4D(this.mouseX, this.mouseY));
                    objetoNovo.PontosAdicionar(new Ponto4D(this.mouseX, this.mouseY));
                }
            }
            else if (e.Mouse.RightButton.Equals(ButtonState.Pressed))
            {
                if (objetoSelecionado != null && this.verticeMaisProximo == null)
                {
                    this.objetosLista.Remove(objetoSelecionado);
                    objetoSelecionado = null;
                }
                else if (objetoSelecionado != null && this.verticeMaisProximo != null)
                {
                    Poligono poligono = (Poligono)objetoSelecionado;
                    if (poligono.getPontosPoligono().Count > 3)
                    {
                        poligono.getPontosPoligono().Remove(this.verticeMaisProximo);
                        this.objetoSelecionado = null;
                        this.verticeMaisProximo = null;
                    }
                    else
                    {
                        this.objetosLista.Remove(objetoSelecionado);
                        objetoSelecionado = null;
                    }
                }
            }
            else
            {
                Console.WriteLine("Número: ");
                string consoleInput = Console.ReadLine();
                int nVertices = Convert.ToInt32(consoleInput);
                objetoNovo = new Poligono(objetoId + 1, null);
                int cont = 360 / nVertices;
                for (int i = 0; i < 360; i += cont)
                {
                    double degInRad = i * 3.1416 / 180;
                    objetoNovo.PontosAdicionar(new Ponto4D((Math.Cos(degInRad) * 50) + xPointClick, (Math.Sin(degInRad) * 50) + yPointClick));
                }
                objetosLista.Add(objetoNovo);
                objetoSelecionado = objetoNovo;
            }
        }
        private void Sru3D()
        {
            GL.LineWidth(1);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(OpenTK.Color.Red);
            GL.Vertex3(300, 300, 0); GL.Vertex3(600, 300, 0);
            GL.Color3(OpenTK.Color.Green);
            GL.Vertex3(300, 300, 0); GL.Vertex3(300, 600, 0);
            GL.Color3(OpenTK.Color.Blue);
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
            GL.End();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG-N3";
            window.Run(1.0 / 60.0);
        }
    }
}