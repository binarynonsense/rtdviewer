/*---------------------------------------------------------------------------
--                              rtdViewer                                  -- 
-----------------------------------------------------------------------------
--                                                                         --
--                                                                         --
--  author:   Álvaro García                                                --
--  website:  www.binarynonsense.com                                       --
--                                                                         --
--  file: main.cs                                                          --
--                                                                         --
--                                                                         --
-----------------------------------------------------------------------------
--                      last update: 29 Jan 2009                           --
---------------------------------------------------------------------------*/

/*
gmcs main.cs windows.cs graphs.cs data.cs -pkg:gtk-sharp-2.0 -r:System.Data.dll -r:Mono.Cairo.dll -r:Mono.Data.SqliteClient.dll -out:rtdViewer.exe

*/
using System;
using Gtk;
using AGC;

public class rtdReader{

                public static Gtk.Window w;
	
		static void Main(string[] args){		
			
        		//DataFunctions dataFunctions = new DataFunctions();        		
        		if(args.Length>0){
        			DataFunctions.dataBaseFileName=args[0];
        		}
        		
        		DataFunctions.Init();

        		Application.Init ();
        		
        		w = new mainWindow();
        		w.SetIconFromFile ("icon.png");
                        w.DeleteEvent += close_window;                        
        		w.ShowAll ();
        		//w.Resize (990, 450);
        						
        		Application.Run ();
        		
		}//main
		
		static void close_window (object obj, DeleteEventArgs args){
			Application.Quit ();
		}
}
