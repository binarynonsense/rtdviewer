/*---------------------------------------------------------------------------
--                              rtdViewer                                  -- 
-----------------------------------------------------------------------------
--                                                                         --
--                                                                         --
--  author:   Álvaro García                                                --
--  website:  www.binarynonsense.com                                       --
--                                                                         --
--  file: windows.cs                                                       --
--                                                                         --
--                                                                         --
-----------------------------------------------------------------------------
--                      last update: 29 Jan 2009                           --
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using Gtk;
//using System.Text;
using System.IO;

namespace AGC
{
        
	public class mainWindow : Gtk.Window
	{
                public TextView view;
        	public TextBuffer buffer;
        	public TextView viewSource;
        	public TextBuffer bufferSource;
        	public DrawingArea a;
        	public ComboBox combo;
        	public Statusbar sb;
        	public const int id = 1;
        	public TextTagTable textTagTable;
        	public TextTag wcet_tag;
        	
        	public ListStore store;
        	public ListStore storeTree;
        	
        	public TreeView treeView;
        	public ScrolledWindow sw2;
        	public ScrolledWindow sw3;
        	public VBox vboxLeft;
        	
        	        	        		
		public mainWindow () : base ("rtdViewer - Developed by Alvaro Garcia")
		{
			//icon:
			//http://www.go-mono.com/docs/monodoc.ashx?link=T%3aGdk.Pixbuf
			
			
			VBox vboxMain = new VBox (false, 0);
			Add (vboxMain);	
			
			AccelGroup aGroup = new AccelGroup();
                        AddAccelGroup(aGroup);	
                        
                        KeyPressEvent += MyKeyPressedEventHandler;	
			
			/////////menu////////////////////			
			MenuBar menubar = new MenuBar ();
			vboxMain.PackStart (menubar, false, true, 0);//widget,expand,fill,padding
						
			MenuItem menuitem = new MenuItem ("_File");			
			Menu menu = new Menu ();
			//MenuItem menuitem2 = new MenuItem (String.Format ("Open"));
			MenuItem menuitem2 = new ImageMenuItem(Stock.Open, aGroup);
			menuitem2.Activated+= OpenRtd;
			menu.Append (menuitem2);			
			//menuitem2.Sensitive = false;
			menuitem2 = new ImageMenuItem(Stock.Quit, aGroup);
                        menuitem2.Activated += new EventHandler(FileQuit_Activated);
                        menu.Append (menuitem2);
                        
			menuitem.Submenu = menu;
			menubar.Append (menuitem);
			
			
			//----------
			
			menuitem = new MenuItem ("_Edit");			
			menu = new Menu ();
			
			menuitem2 = new MenuItem (String.Format ("Units"));
			menu.Append (menuitem2);
			menuitem.Submenu = menu;			
			menubar.Append (menuitem);
			
			Menu menu2 = new Menu ();
			MenuItem menuitem3 = new MenuItem (String.Format ("miliseconds"));
			menuitem3.Activated+= new EventHandler (ChangeUnitMiliseconds);
			menuitem2.Submenu = menu2;
			menu2.Append (menuitem3);
			
			menuitem3 = new MenuItem (String.Format ("microseconds"));
			menuitem3.Activated+= new EventHandler (ChangeUnitMicroseconds);
			menu2.Append (menuitem3);
			
			menuitem3 = new MenuItem (String.Format ("nanoseconds"));
			menuitem3.Activated+= new EventHandler (ChangeUnitNanoseconds);
			menu2.Append (menuitem3);
			
			//**
						
			menuitem2 = new MenuItem (String.Format ("Graph Colors"));
			menu.Append (menuitem2);			
			menu2 = new Menu ();
			menuitem3 = new MenuItem (String.Format ("Gray"));
			menuitem3.Activated+= new EventHandler (ChangeGraphColorsGray);
			menuitem2.Submenu = menu2;
			menu2.Append (menuitem3);
			
			menuitem3 = new MenuItem (String.Format ("Blue"));
			menuitem3.Activated+= new EventHandler (ChangeGraphColorsBlue);
			menu2.Append (menuitem3);
			
			//**	
			menuitem2 = new MenuItem (String.Format ("Save Graph as PNG"));
			menuitem2.Activated+= SaveGraphHandler;
			menu.Append (menuitem2);
			//**	
			menuitem2 = new MenuItem (String.Format ("Take Screenshot (F5)"));
			menuitem2.Activated+= ScreenshotHandler;
			menu.Append (menuitem2);
			
			
			//----------
			menuitem = new MenuItem ("_Help");			
			menu = new Menu ();
			menuitem2 = new ImageMenuItem(Stock.Help, aGroup);
			menuitem2.Activated+= new EventHandler (HelpDialogClicked);
			menu.Append (menuitem2);
			menuitem2 = new ImageMenuItem(Stock.About, aGroup);
			menuitem2.Activated+= new EventHandler (MessageDialogClicked);
			menu.Append (menuitem2);			
			menuitem.Submenu = menu;
			menubar.Append (menuitem);			
			/////////////////////////////////

			HBox hboxMain = new HBox (false, 5);
			hboxMain.BorderWidth=3;
			/*
			    homogeneous: If true, all widgets in the box are forced to be equally sized.
                            spacing: The number of pixels to place between each widget in the box.
			*/
			vboxMain.PackStart (hboxMain, true, true, 0);//widget,expand,fill,padding					
			
			//*******************************************************************
			
        		
        		VBox vboxRight = new VBox (false, 4);
        		Alignment alignmentRight = new Alignment(0,0,0,1);			
			alignmentRight.Add (vboxRight);
			hboxMain.PackStart (alignmentRight, false, false, 0);//widget,expand,fill,padding
			//hboxMain.Add (alignmentRight);
			//hboxMain.Add (vboxRight);
			
			
			///////////////CHOOSE FUNCTION///////////////						
			Frame frameFunction = new Frame("Function");
                        //vboxRight.Add(frameFunction);
                        vboxRight.PackStart (frameFunction, false, false, 0);//widget,expand,fill,padding
                        frameFunction.Label = "Function/Procedure:";
                        frameFunction.LabelXalign=(float)0.02;
                        //frame.ShadowType = (ShadowType) 4;
                        frameFunction.Show();
        		
        		combo = new ComboBox();
        		combo.BorderWidth=5;
        		
        		ListStore store = new ListStore(typeof(string)); 
                        CellRendererText text = new CellRendererText(); 
                        combo.PackStart(text, false); 
                        combo.AddAttribute(text, "text", 0);
                        combo.Model =store; 
                        
                        for(int i=0;i<DataFunctions.validIDs.Count;i++){
                                store.AppendValues(DataFunctions.ids2Names[DataFunctions.validIDs[i].ToString()]);			
			} 			
                        combo.Active=0;  
                        combo.Changed += new EventHandler (OnComboBoxChanged);
        		frameFunction.Add (combo);        		
        		///////////////////////////// 
			
			////////TEXT VIEW DATA//////
        		Frame frameData = new Frame("Execution Time Data Frame");
        		//frameData.BorderWidth=5;
                        vboxRight.Add(frameData);
                        frameData.Label = "Execution Time Data:";
                        frameData.LabelXalign=(float)0.02;
                        //frame.ShadowType = (ShadowType) 4;
                        frameData.Show();	

        		view = new TextView ();
        		view.BorderWidth=5;
        		buffer = view.Buffer;
        		UpdateTextAreaBuffer();
        		view.Editable=false;
        		view.LeftMargin=5;
        		frameData.Add (view);
        		////////////////////////
        		
        		///////TREEVIEW SCROLLED
        		// create model
        		Frame frameTree = new Frame("Source Code Blocks");
                        frameTree.Label = "Source Code Blocks:";
                        frameTree.LabelXalign=(float)0.02;
                        frameTree.Show();
                        vboxRight.Add (frameTree);
                        
        		sw3 = new ScrolledWindow ();
			sw3.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw3.ShadowType = ShadowType.In;
			sw3.BorderWidth = 4;
			frameTree.Add (sw3);
			storeTree = CreateModel ();
			// create tree view			
			treeView = new TreeView (storeTree);
			treeView.RulesHint = true;
			treeView.SearchColumn = (int) Column.InitLine;
			sw3.Add (treeView);
			AddColumns (treeView);			
			treeView.RowActivated += new RowActivatedHandler(OnTreeRowActivated);
			//////////////////////////
			
			vboxLeft = new VBox (false, 5);
			Alignment alignmentLeft = new Alignment(1,1,1,1);			
			alignmentLeft.Add (vboxLeft);
			//hboxMain.Add (alignmentLeft);
			hboxMain.PackEnd (alignmentLeft, true, true, 0);//widget,expand,fill,padding
			//hboxMain.Add (vboxLeft);
			//hboxMain.PackStart (vboxLeft, false, false, 0);//widget,expand,fill,padding
						
			/////////SCROLLW GRAPHIC/////////////////
			
			Frame frameGraph = new Frame("End-to-End Graph");
                        frameGraph.Label = "End-to-End Graph:";
                        frameGraph.LabelXalign=(float)0.02;
                        frameGraph.Show();
                        vboxLeft.Add (frameGraph);
                        
			ScrolledWindow sw = new ScrolledWindow ();
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw.ShadowType = ShadowType.In;
			sw.BorderWidth=3;			
			frameGraph.Add (sw);
			/*
                                child: A widget to pack into the box.
                                expand: If true, the child widget will expand to use as much space as it is given.
                                fill: If true, the child widget will request as much space as is available.
                                padding: The size (in pixels) of a border to place around the specified child widget.
                        */			
			a = new CairoGraphic ();
			sw.AddWithViewport (a);	
			/////////////////////////////////////////
			
			///TEXT VIEW SOURCE//////////////////////
			
			Frame frameSource = new Frame("Source Code");
                        frameSource.Label = "Source Code:";
                        frameSource.LabelXalign=(float)0.02;
                        frameSource.Show();
                        vboxLeft.Add (frameSource);
                        
        		sw2 = new ScrolledWindow ();
			sw2.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw2.ShadowType = ShadowType.In;
			sw2.BorderWidth = 5;
			frameSource.Add (sw2);
			
        		viewSource = new TextView ();
        		//viewSource.BorderWidth=5;
        		bufferSource = viewSource.Buffer;
        		
        		// create text tag table
			/*textTagTable = new TextTagTable ();
			wcet_tag = new TextTag ("wcet");
			wcet_tag.Foreground = "blue";
			wcet_tag.Style = Pango.Style.Normal;
			textTagTable.Add (wcet_tag);
			
			TextTag tag  = new TextTag ("heading");
                        tag.Weight = Pango.Weight.Bold;
                        tag.Size = (int) Pango.Scale.PangoScale * 15;
                        buffer.TagTable.Add (tag);*/
                        wcet_tag  = new TextTag ("wcet");
                        wcet_tag.Foreground = "yellow";
                         wcet_tag.Background = "blue";
			wcet_tag.Style = Pango.Style.Normal;
                        bufferSource.TagTable.Add (wcet_tag);
                        
        		UpdateTextAreaSourceBuffer();
        		viewSource.Editable=false;
        		viewSource.LeftMargin=5;       		
        		sw2.Add (viewSource);
        		//TEMP
        		//sw2.HscrollbarPolicy=Gtk.PolicyType.Always;
        		//END TEMP
        		//////////////////////////////////////
        		
			//*******************************************************************
			
			///STATUS BAR///
        		sb = new Statusbar ();
        		sb.Push (id, "rtdViewer Version: "+DataFunctions.version+" :: Rapitime Version: " + DataFunctions.rapitimeVersion+" :: Units: "+DataFunctions.unitString);
        		//sb.HasResizeGrip = true;
        		//vboxMain.Add (sb);
        		vboxMain.PackStart (sb, false, false, 0);//widget,expand,fill,padding
        		///////////////        		
        		
        		
        		/////////RESIZE ////////////////////
        		
                        //left column
			combo.SetSizeRequest(240,30);
			view.SetSizeRequest(240,125);
			sw3.SetSizeRequest(240,260);

			
			//right column
			sw.SetSizeRequest(700,260);
			sw2.SetSizeRequest(700,165);
			
			//sb.SetSizeRequest(685,20);
			
			
			//AllowGrow=false;
					

			ShowAll ();
		}//end mainwindow constructor
		
		//////////////////////////////////////////////////////////////////////////
		
		void OnComboBoxChanged (object o, EventArgs args)
	        {
        		ComboBox combo = o as ComboBox;
        		if (o == null)
        			return;

        		TreeIter iter;

        		if (combo.GetActiveIter (out iter)){
        		
        			//Console.WriteLine ((string) combo.Model.GetValue (iter, 0));
        			//buffer.Text=(string) combo.Model.GetValue (iter, 0);
        			//DataFunctions.functionID = "15";
        			string name = (string) combo.Model.GetValue (iter, 0);
        			DataFunctions.functionID = DataFunctions.NameToID(name);
        			DataFunctions.functionName = DataFunctions.ids2Names[DataFunctions.functionID];
        			
        			DataFunctions.DBOpen();           
                		DataFunctions.DBExtractFunctionData();                		
                		DataFunctions.DBExtractEndToEndData(); 
                		DataFunctions.DBExtractSourceData(); 
                		DataFunctions.DBExtractTransitionsData();              		
        		        DataFunctions.DBClose();
        		        
        		        UpdateTextAreaBuffer();
        		        UpdateTextAreaSourceBuffer();
        		
        		        
        		}
        		
        		ReloadView();        		

        		
        		
	        }//end comboboxchanged
	        
	        private void OpenRtd (object o, EventArgs args){
	                //http://www.go-mono.com/docs/index.aspx?link=T%3AGtk.FileChooserDialog
	                Gtk.FileChooserDialog fc= new Gtk.FileChooserDialog("Choose the file to open",
		                            this,
		                            FileChooserAction.Open,
		                            "Cancel",ResponseType.Cancel,
		                            "Open",ResponseType.Accept);
		                            
                        FileFilter filter = new FileFilter();
        		filter.Name = "rtd files";
        		filter.AddPattern("*.rtd");
        		fc.AddFilter(filter);

        		if (fc.Run() == (int)ResponseType.Accept) 
        		{
        			//System.IO.FileStream file=System.IO.File.OpenRead(fc.Filename);
        			//file.Close();
        			DataFunctions.dataBaseFileName=fc.Filename;
        		}
        		//Don't forget to call Destroy() or the FileChooserDialog window won't get closed.
        		fc.Destroy();
        		//Reload All
        		
        		DataFunctions.functionID = "1";
        		
        		
	                
        		DataFunctions.Init();
        		
        		//treeView = new TreeView (store2);
        		
        		UpdateTextAreaBuffer();
        		UpdateTextAreaSourceBuffer();
        		
        		
        		combo.Clear();
        		ListStore store = new ListStore(typeof(string)); 
                        CellRendererText text = new CellRendererText(); 
                        combo.PackStart(text, false); 
                        combo.AddAttribute(text, "text", 0);
                        combo.Model =store;
                        
                        for(int i=0;i<DataFunctions.validIDs.Count;i++){
                                store.AppendValues(DataFunctions.ids2Names[DataFunctions.validIDs[i].ToString()]);			
			} 			
                        combo.Active=0;
                        
                        
                        UpdateTreeView();
                        
                        
                        UpdateStatusBarMessage("rtdViewer Version: "+DataFunctions.version+" :: Rapitime Version: " + DataFunctions.rapitimeVersion+" :: Units: "+DataFunctions.unitString);
                        
                                		
        		ReloadView();
        		
		}//end OpenRtd
		
		public void FileQuit_Activated(object o, EventArgs args){
		        Application.Quit ();
		}//end FileQuit_Activated
		
		private void ReloadView(){
		        a.HideAll();
        		a.ShowAll();
        		combo.HideAll();
        		combo.ShowAll(); 
        		//change windowname
        		rtdReader.w.Title  = DataFunctions.dataBaseFileName + " :: rtdViewer - Developed by Alvaro Garcia";     
        		UpdateStatusBarMessage("rtdViewer Version: "+DataFunctions.version+" :: Rapitime Version: " + DataFunctions.rapitimeVersion+" :: Units: "+DataFunctions.unitString); 
        		UpdateTreeView(); 
        		UpdateTextAreaBuffer();		
        		        		
		}//end ReloadView
		
		private void MessageDialogClicked (object o, EventArgs args)
		{
			using (Dialog dialog = new MessageDialog (this,
								  DialogFlags.Modal | DialogFlags.DestroyWithParent,
								  MessageType.Info,
								  ButtonsType.Ok,
								  "Version: "+DataFunctions.version+"\nLast Update: "+DataFunctions.lastUpdate+"\n\n(c)2009 Alvaro Garcia Cuesta\n\n")) {
				dialog.Run ();
				dialog.Hide ();
			}
			
		}//end MessageDialogClicked
		
		private void HelpDialogClicked (object o, EventArgs args)
		{
			using (Dialog dialog = new MessageDialog (this,
								  DialogFlags.Modal | DialogFlags.DestroyWithParent,
								  MessageType.Info,
								  ButtonsType.Ok,
								  "\n- Double-click on a block's row to highlight it's code in the source code view\n\n- Press F5 to take a screenshot\n\n")) {
				dialog.Run ();
				dialog.Hide ();
			}
			
		}//end HelpDialogClicked
		
		private void UpdateStatusBarMessage(string message){
		        sb.Pop (id);
		        sb.Push (id, message);
		}//end UpdateStatusBarMessage
		
		private void UpdateTextAreaBuffer(){
		        buffer.Text = 
        		"\nmax measured: " + DataFunctions.met_max + " " + DataFunctions.unitStringShort+ "\n" +  
        		"min measured: " + DataFunctions.met_min + " " + DataFunctions.unitStringShort+ "\n" + 
        		"avg measured: " + DataFunctions.met_avg + " " + DataFunctions.unitStringShort+ "\n" + 
        		"WCET: " + (DataFunctions.self_wcet+DataFunctions.sub_wcet)+ " " + DataFunctions.unitStringShort+ "\n";
		}//end UpdateTextAreaBuffer
		
		private void UpdateTextAreaSourceBuffer(){
		//text tags: http://lists.ximian.com/pipermail/gtk-sharp-list/2004-July/004484.html
		        //bufferSource.Text =DataFunctions.functionSource;
		        bufferSource.Text="";
		        TextIter insertIter = bufferSource.EndIter;
		        //TextIter showIter= bufferSource.EndIter;
		        //TextMark mark = null;
		        bool started=false;
		        //bufferSource.Insert (ref insertIter, "hola\n");
		        //insertIter = bufferSource.EndIter;
		        //bufferSource.InsertWithTagsByName (ref insertIter, "asasasasa", "wcet");
		        StringReader strReader = new StringReader(DataFunctions.functionSource);
                		int line=1;
                		string aLine;
                                while(strReader.Peek( ) != -1)
                                {
                                        insertIter = bufferSource.EndIter;
                                        aLine = strReader.ReadLine();
                                        int lineNumber=line-1+DataFunctions.sourceInitLine;
                                        if(line>=DataFunctions.transitionSourceInitLine-DataFunctions.sourceInitLine+1 && line<=DataFunctions.transitionSourceEndLine-DataFunctions.sourceInitLine+1){
                                                if(!started){
                                                        //showIter = insertIter;
                                                        started=true;
                                                        //mark=bufferSource.GetMark("markInit") ;
                                                }
                                                bufferSource.InsertWithTagsByName (ref insertIter, lineNumber+": "+aLine+"\n", "wcet");
                                        }else{
                                                bufferSource.Insert (ref insertIter, lineNumber+": "+aLine+"\n");
                                        }
                                        
                                        line++;
                                }
		        //Console.WriteLine(DataFunctions.functionSource);
		        //view.ScrollToIter (showIter, 0.0, false, 0.0, 0.0);
		        //public TextIter GetIterAtLine (int line_number)
		        //public void ScrollMarkOnscreen (TextMark mark)
		        //view.ScrollMarkOnscreen (mark);
		}//end UpdateTextAreaSourceBuffer
		
		//////////////tree///////////////////
		private void AddColumns (TreeView treeView)
		{

			// column for bug numbers
			CellRendererText rendererText = new CellRendererText ();
			TreeViewColumn column = new TreeViewColumn ("Init line", rendererText, "text", Column.InitLine);
			column.SortColumnId = (int) Column.InitLine;
			treeView.AppendColumn (column);
			
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("End line", rendererText, "text", Column.EndLine);
			column.SortColumnId = (int) Column.EndLine;
			treeView.AppendColumn (column);

			// column for severities
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("Wcet", rendererText, "text", Column.Wcet);
			column.SortColumnId = (int)Column.Wcet;
			treeView.AppendColumn(column);
			
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("ID", rendererText, "text", Column.ID);
			column.SortColumnId = (int)Column.ID;
			treeView.AppendColumn(column);

		}


		private ListStore CreateModel ()
		{
			ListStore store = new ListStore (typeof(int),
			                                 typeof(int),
							 typeof(double),
							 typeof(int));

			//store.AppendValues (1,8,13.5);			
			
			/*foreach (KeyValuePair<int, DataFunctions.TransitionDataStruct> transitionData in DataFunctions.transitionsData){
			
			        
			   store.AppendValues (transitionData.Value.initLine,transitionData.Value.endLine,transitionData.Value.wcet,transitionData.Key);

                       }*/
                       foreach (DataFunctions.TransitionDataStruct transitionData in DataFunctions.transitionsData){
			
			        
			   store.AppendValues (transitionData.initLine,transitionData.endLine,transitionData.wcet,transitionData.xrefID);

                       }
			

			return store;
		}

		private enum Column
		{
			InitLine,
			EndLine,
			Wcet,
			ID
		}
		
		private void UpdateTreeView(){
			
        		storeTree.Clear();	        
        		                		
                	foreach (DataFunctions.TransitionDataStruct transitionData in DataFunctions.transitionsData){			
        			        
        		        storeTree.AppendValues (transitionData.initLine,transitionData.endLine,transitionData.wcet,transitionData.xrefID);
                        }
        		
		}//end UpdateTreeView
		
		private void OnTreeRowActivated(object o, EventArgs args){
		
		

		        TreeView tv = (TreeView) o;
		        RowActivatedArgs args2 = (RowActivatedArgs) args;

                        TreeModel model = tv.Model;
                        TreeIter iter;
                        model.GetIter (out iter, args2.Path);
                        GLib.Value val = new GLib.Value();
                        model.GetValue (iter, 3, ref val);
		        
		        foreach (DataFunctions.TransitionDataStruct transitionData in DataFunctions.transitionsData){			
        			 if(transitionData.xrefID==(int)val.Val){
        			        DataFunctions.transitionSourceInitLine=transitionData.initLine;
        			        DataFunctions.transitionSourceEndLine=transitionData.endLine;		        
                                        
                                        break;
        			 }       
                        }
                        
                        UpdateTextAreaSourceBuffer();
                        //sw2.Vadjustment.Value=10;
                        //Console.WriteLine("scrolled: "+sw2.Vadjustment.Value);
                        
                        //sw2.Vadjustment.Value=sw2.Vadjustment.Upper;
                        //Console.WriteLine("upper: "+sw2.Vadjustment.Upper);
                        //lines of code
                        int numberOfLines = DataFunctions.sourceEndLine-DataFunctions.sourceInitLine+1;
                        int lineNumber = DataFunctions.transitionSourceInitLine-DataFunctions.sourceInitLine+1;
                        
                        //double upper=0.0;
                        double theValue=0.0;
                        
                        
                        //if(numberOfLines>10){//might have to calculate the number of lines (1 line = 17.09 pixels?)
                        if(numberOfLines>(sw2.Vadjustment.PageSize/17.09)){
                                //upper = sw2.Vadjustment.Upper;
                                
                                //sw2.Vadjustment.StepIncrement / sw2.Vadjustment.PageSize / sw2.Vadjustment.PageIncrement
                                                                
                                theValue = (  (sw2.Vadjustment.Upper)/(double)(numberOfLines+1)  )*(double)(lineNumber-2);
                                
                                //Console.WriteLine("upper: "+sw2.Vadjustment.Upper);
                        
                        }else{
                               theValue=0.0; 
                        }
                        sw2.Vadjustment.Value=theValue;    
                        
                        //vboxLeft.SetSizeRequest(260,-1);;                 
                        
		        
		}//OnTreeRowActivated
		/////end tree
		
		//should be just one function, but I don't know how to get the label name from the menuitem object
		public void ChangeGraphColorsBlue(object o, EventArgs args){
		        //(0.27,0.42,0.96));//light blue
		        //(0.05,0.23,0.87));//dark blue
		        DataFunctions.graphColors.r1=0.27;
		        DataFunctions.graphColors.g1=0.42;
		        DataFunctions.graphColors.b1=0.96;
			DataFunctions.graphColors.r2=0.05;
			DataFunctions.graphColors.g2=0.23;
			DataFunctions.graphColors.b2=0.87;
			a.HideAll();
        		a.ShowAll();
        		bufferSource.TagTable.Remove (wcet_tag);
        		wcet_tag  = new TextTag ("wcet");
                        wcet_tag.Foreground = "yellow";
                        wcet_tag.Background = "blue";
			wcet_tag.Style = Pango.Style.Normal;
                        bufferSource.TagTable.Add (wcet_tag);
                        
        		UpdateTextAreaSourceBuffer();
		        
		}////end ChangeGraphColorsBlue
		public void ChangeGraphColorsGray(object o, EventArgs args){
		        //(0.63,0.63,0.64));//light grey
			//(0.51,0.51,0.52));//dark grey
			DataFunctions.graphColors.r1=DataFunctions.graphColors.g1=DataFunctions.graphColors.b1=0.63;
			DataFunctions.graphColors.r2=DataFunctions.graphColors.g2=DataFunctions.graphColors.b2=0.52;
			a.HideAll();
        		a.ShowAll();
        		bufferSource.TagTable.Remove (wcet_tag);
        		wcet_tag  = new TextTag ("wcet");
                        wcet_tag.Foreground = "white";
                        wcet_tag.Background = "gray";
			wcet_tag.Style = Pango.Style.Normal;
                        bufferSource.TagTable.Add (wcet_tag);
                        
        		UpdateTextAreaSourceBuffer();
		        
		}////end ChangeGraphColorsGray
		
		
		//should be just one function, but I don't know how to get the label name from the menuitem object
		public void ChangeUnitMiliseconds(object o, EventArgs args){
		        ChangeUnit("miliseconds");
		}////end ChangeUnitMiliseconds
		public void ChangeUnitMicroseconds(object o, EventArgs args){
		        ChangeUnit("microseconds");
		}////end ChangeUnitMicroseconds
		public void ChangeUnitNanoseconds(object o, EventArgs args){
		        ChangeUnit("nanoseconds");
		}////end ChangeUnitNanoseconds
		
		public void ChangeUnit(string newUnit){
		
		        double unitBefore = DataFunctions.unit;;
		
		        switch(newUnit){
		                case "miliseconds":
		                        DataFunctions.unit = DataFunctions.unitMiliSec;
                        	        DataFunctions.unitString = DataFunctions.unitStringMiliSec;
                        	        DataFunctions.unitStringShort = DataFunctions.unitStringShortMiliSec;
                        	        break;
                        	case "microseconds":
		                        DataFunctions.unit = DataFunctions.unitMicroSec;
                        	        DataFunctions.unitString = DataFunctions.unitStringMicroSec;
                        	        DataFunctions.unitStringShort = DataFunctions.unitStringShortMicroSec;
                        	        break;
                        	case "nanoseconds":
		                        DataFunctions.unit = DataFunctions.unitNanoSec;
                        	        DataFunctions.unitString = DataFunctions.unitStringNanoSec;
                        	        DataFunctions.unitStringShort = DataFunctions.unitStringShortNanoSec;
                        	        break;
                        	default:
                        	        break;
		        }
		
		        //double unitBefore = DataFunctions.unit;
		        //DataFunctions.unit = DataFunctions.unitMiliSec;
        	        //DataFunctions.unitString = DataFunctions.unitStringMiliSec;
        	        
        	        DataFunctions.factor=DataFunctions.factor*DataFunctions.unit/unitBefore;
        	        DataFunctions.met_max *= unitBefore/DataFunctions.unit;
        		DataFunctions.met_min *= unitBefore/DataFunctions.unit;
        		DataFunctions.self_met_total *= unitBefore/DataFunctions.unit;
        		DataFunctions.self_wcet *= unitBefore/DataFunctions.unit;			
        		DataFunctions.sub_met_total *= unitBefore/DataFunctions.unit;
        		DataFunctions.sub_wcet *= unitBefore/DataFunctions.unit;		
        		DataFunctions.met_avg *= unitBefore/DataFunctions.unit;
        		
        		int numExecutions=DataFunctions.endToEnd.Count;
        		for(int k=0;k<numExecutions;k++){
		
			        DataFunctions.endToEnd[k]=Convert.ToDouble(DataFunctions.endToEnd[k])*unitBefore/DataFunctions.unit;
			}
			
			//not the most efficient way :P
			DataFunctions.DBOpen();
			DataFunctions.DBExtractTransitionsData();
			DataFunctions.DBClose();
			UpdateTreeView();
				
        	        
        	        ReloadView();		        
		}////end ChangeUnitMiliseconds
		
		void SaveGraphHandler(object o, EventArgs args){
		        //CairoGraphic b = (CairoGraphic)a;
		        Gdk.Rectangle area = a.Allocation;
			//Console.WriteLine(area.Height+"x"+area.Width);
			string fileName="graph";
			int i=0;
			while(true){
			        if(!File.Exists (fileName+"_"+i+".png")){
			                break;
			        }
			        i++;
			}
			((CairoGraphic)a).SaveGraph(area.Width,area.Height,fileName+"_"+i);
		}////end SaveGraphHandler
		
		void ScreenshotHandler(object o, EventArgs args){
		        //this.ChildFocus(Gtk.DirectionType.Up);
		        Screenshot("png");    
		}//end Screenshot
		
		public void MyKeyPressedEventHandler(object o, KeyPressEventArgs args){
		        
		        if(args.Event.Key==Gdk.Key.F5){
		                Screenshot("png"); 
		        }
		        
		}//end MyKeyPressEventHandler
		
		void Screenshot(string extension){//jpeg and png supported
		        //http://lists.ximian.com/pipermail/gtk-sharp-list/2007-April/007961.html
		        // holder variables
                        int width = 0;
                        int height = 0;

                        // get the root window
                        // NOTE: if you have multiple monitors, this may not grab the correct root
                        //window
                        // or it may grab the entire desktop if it is stretched over multiple
                        //screens (a la xinerama)
                        // there are ways to enumerate the screens and pick the one you want...
                        //Gdk.Window root = Gdk.Global.DefaultRootWindow;
                        Gdk.Window root = Gdk.Global.ActiveWindow;

                        // get its width and height
                        root.GetSize(out width, out height);

                        // create a new pixbuf from the root window
                        Gdk.Pixbuf screenshot = Gdk.Pixbuf.FromDrawable(root, root.Colormap, 0, 0, 0, 0, width, height);

                        // save it
                        string fileName="screenshot";
			int i=0;
			while(true){
			        if(!File.Exists (fileName+"_"+i+"." + extension)){
			                break;
			        }
			        i++;
			}
                        screenshot.Save(fileName+"_"+i+"." + extension, extension);
		}//end Screenshot
 
	}//end main window

}//end namespace
