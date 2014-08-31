/*---------------------------------------------------------------------------
--                              rtdViewer                                  -- 
-----------------------------------------------------------------------------
--                                                                         --
--                                                                         --
--  author:   Álvaro García                                                --
--  website:  www.binarynonsense.com                                       --
--                                                                         --
--  file: graphs.cs                                                        --
--                                                                         --
--                                                                         --
-----------------------------------------------------------------------------
--                      last update: 29 Jan 2009                           --
---------------------------------------------------------------------------*/

using System;
using Cairo;
using Gtk;
using System.Collections;


namespace AGC
{

	public class CairoGraphic : DrawingArea
	{

	    protected override bool OnExposeEvent (Gdk.EventExpose args)
	    {
		using (Cairo.Context g = Gdk.CairoHelper.Create (args.Window)){
			DrawGraph(g);
		}
	        return true;
	    }
	    
	    public void SaveGraph (int width,int height,string fileName){
        	using (ImageSurface draw = new ImageSurface (Format.Argb32, width, height)){
        	    using (Context gr = new Context(draw)){
                        DrawGraph(gr);                        
                        draw.WriteToPng (fileName+".png");	//save the image as a png image.
                    }
        	}
            }
	    
	    public void DrawGraph(object g1){
	        Cairo.Context g = (Cairo.Context)g1;
	        double maxExecutionValue=0;
			for(int i=0;i<DataFunctions.endToEnd.Count;i++){
				if(Convert.ToDouble(DataFunctions.endToEnd[i])>maxExecutionValue){
					maxExecutionValue=Convert.ToDouble(DataFunctions.endToEnd[i]);
				}
			}
			
			
			//int areaWidth=685;
			//int areaHeight=300;
			
			Gdk.Rectangle area = this.Allocation;
			//Console.WriteLine(area.Height+"x"+area.Width);
			int areaWidth=area.Width;
			int areaHeight=area.Height;
			int areaPaddingVert=50;	
			int areaPaddingHorzIzda=70;	
			int areaPaddingHorzDcha=30;	
			
						
			int maxHeight=areaHeight-2*areaPaddingVert;
			int plotBaseHeight=maxHeight+areaPaddingVert;
			int maxWidth=areaWidth - areaPaddingHorzIzda - areaPaddingHorzDcha;
			int numExecutions=DataFunctions.endToEnd.Count;
			
						
			//space between columns
			double columnsSpace = (double)maxWidth/(double)numExecutions;//column+space at its right
			double columnsWidth = (double)columnsSpace*0.8;

		
			g.Color = new Color(255, 255, 255);
			g.Paint();
			
			int height;


			for(int k=0;k<numExecutions;k++){
		
				height=(int)((Convert.ToDouble(DataFunctions.endToEnd[k])/maxExecutionValue)*maxHeight  );//num*maxHeight/max			

				
				g.Save ();
				Cairo.Gradient pat = new Cairo.LinearGradient (200, 100, 200, 200);//grad from x0 y0 to x1 y1
				//pat.AddColorStop (0, new Cairo.Color (0.27,0.42,0.96));//light blue
				//pat.AddColorStop (1, new Cairo.Color (0.05,0.23,0.87));//dark blue
				//pat.AddColorStop (0, new Cairo.Color (0.63,0.63,0.64));//light grey
				//pat.AddColorStop (1, new Cairo.Color (0.51,0.51,0.52));//dark grey (or the other way around?)
				pat.AddColorStop (0, new Cairo.Color (DataFunctions.graphColors.r1,DataFunctions.graphColors.g1,DataFunctions.graphColors.b1));//light blue
				pat.AddColorStop (1, new Cairo.Color (DataFunctions.graphColors.r2,DataFunctions.graphColors.g2,DataFunctions.graphColors.b2));//dark blue
				//DataFuncions.graphColors.r1
				g.Pattern = pat;
				g.Rectangle(areaPaddingHorzIzda+(columnsSpace*k), plotBaseHeight-height, columnsWidth, height);				
				g.Fill ();				
				g.Restore ();								
					
			}			
			
			//draw axis			
			//x
			g.Antialias = Antialias.Subpixel;	
			g.LineWidth = 2;			
			g.Color = new Color (0, 0, 0);	
			g.MoveTo (areaPaddingHorzIzda, plotBaseHeight);			
			g.LineTo (areaWidth-areaPaddingHorzDcha, plotBaseHeight);		
			g.Stroke ();
			//y
			g.MoveTo (areaPaddingHorzIzda, areaPaddingVert-5);			
			g.LineTo (areaPaddingHorzIzda, plotBaseHeight);			
			g.Stroke ();			
			
			//draw red line (max height)
			g.LineWidth = 1;			
			g.Color = new Color (200, 0, 0);	
			g.MoveTo (areaPaddingHorzIzda, areaPaddingVert);			
			g.LineTo (areaWidth-areaPaddingHorzDcha, areaPaddingVert);		
			g.Stroke ();
		
			
			
			//axis labels//////////
			//y///////

			//max
			g.Color = new Color(0, 0, 0);
			g.SelectFontFace("Georgia", FontSlant.Normal, FontWeight.Bold);
			g.SetFontSize(10);
			g.MoveTo(areaPaddingHorzIzda-60, areaPaddingVert);
			g.ShowText(maxExecutionValue.ToString("0.######"));
			
			g.LineWidth = 1;			
			g.MoveTo(areaPaddingHorzIzda-5, areaPaddingVert);			
			g.LineTo (areaPaddingHorzIzda, areaPaddingVert);		
			g.Stroke ();
			
			//med			
			g.MoveTo(areaPaddingHorzIzda-60, areaPaddingVert+maxHeight/2);
			g.ShowText((maxExecutionValue/2).ToString("0.######"));
	
			g.MoveTo(areaPaddingHorzIzda-5, areaPaddingVert+maxHeight/2);			
			g.LineTo (areaPaddingHorzIzda, areaPaddingVert+maxHeight/2);		
			g.Stroke ();
			
			//0		
			g.MoveTo(areaPaddingHorzIzda-20, areaPaddingVert+maxHeight);
			g.ShowText("0");
	
			g.MoveTo(areaPaddingHorzIzda-5, areaPaddingVert+maxHeight);			
			g.LineTo (areaPaddingHorzIzda, areaPaddingVert+maxHeight);		
			g.Stroke ();
			
			//x///////
			
			//1		
			g.MoveTo(areaPaddingHorzIzda+columnsWidth/2, areaPaddingVert+maxHeight+20);
			g.ShowText("1");
	
			g.MoveTo (areaPaddingHorzIzda+columnsWidth/2, plotBaseHeight);				
			g.LineTo (areaPaddingHorzIzda+columnsWidth/2, plotBaseHeight+5);		
			g.Stroke ();
			
			//max		
			g.MoveTo(areaPaddingHorzIzda+(columnsSpace*(numExecutions-1))+columnsWidth/2, areaPaddingVert+maxHeight+20);
			g.ShowText(numExecutions.ToString());
	
			g.MoveTo (areaPaddingHorzIzda+(columnsSpace*(numExecutions-1))+columnsWidth/2, plotBaseHeight);				
			g.LineTo (areaPaddingHorzIzda+(columnsSpace*(numExecutions-1))+columnsWidth/2, plotBaseHeight+5);		
			g.Stroke ();
			
			//TITLE/////////////////

			
			g.MoveTo(areaWidth/2-((DataFunctions.functionName.Length/2))*6, areaPaddingVert/2);
			g.ShowText(DataFunctions.functionName);
			
			//Executions
			
			//g.Color = new Color(0, 0, 255);
			//g.Color = new Color(0.51,0.51,0.52);
			g.Color = new Color(DataFunctions.graphColors.r2,DataFunctions.graphColors.g2,DataFunctions.graphColors.b2);
			
			g.MoveTo(areaWidth/2-4*6, plotBaseHeight+areaPaddingVert/2);
			g.ShowText("executions");
			
			//Time
			
			g.MoveTo(areaPaddingHorzIzda/2-5*6, areaPaddingVert/2);//before : 2*6
			g.ShowText("time (" + DataFunctions.unitStringShort + ")");
	    }
	}
	
}//end namespace

