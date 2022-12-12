using System;
using System.Collections.Generic;
using Mastercam.Database;
using Mastercam.Math;
using Mastercam.IO;
using Mastercam.Database.Types;
using Mastercam.GeometryUtility.Types;
using Mastercam.App.Types;
using Mastercam.GeometryUtility;
using Mastercam.Support;
using Mastercam.Curves;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace _rpGenCut
{
    public class rpGenCut : Mastercam.App.NetHook3App
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        const UInt32 MouseEventLeftDown = 0x0201;
        const UInt32 MouseEventLeftUp = 0x0202;
        public Mastercam.App.Types.MCamReturn rpGenCutRun(Mastercam.App.Types.MCamReturn notused)
        {
            var tempList1 = new List<int>(); // list of x+, y+ entities
            var tempList2 = new List<int>(); // list of x-, y- entitites
            var tempList3 = new List<int>(); // list of starting arcs
            var tempList4 = new List<int>(); // list of arc points
            var tempList5 = new List<Geometry>(); // list is broken arcs
            var tempList6 = new List<Geometry>(); // error list
            var tempList7 = new List<Geometry>(); // arcs to break
            var tempList8 = new List<int>(); // list of starting entities
            var tempList9 = new List<int>(); // list of all points
            var templist10 = new List<int>(); // temporarily holds geoID of chainEntities
            var templist80 = new List<int>();
            var templist81 = new List<int>();
            var upperCreaseID = new List<int>();
            var lowerCreaseID = new List<int>();

            void offsetCutchain80(){
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(80);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown){
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(80, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                int createdUpperLevel = 500;
                int createdLowerLevel = 501;
                LevelsManager.SetLevelName(500, "Upper Created Cut Geo");
                LevelsManager.SetLevelName(501, "Lower Created Cut Geo");

                var commonParams = new BoundingBoxCommonParams { CreateLinesArcs = true };
                var rectangularParams = new BoundingBoxRectangularParams { ExpandXMinus = 2.0, ExpandXPlus = 2.0, ExpandYMinus = 2.0, ExpandYPlus = 2.0 };
                var boundingBox = Mastercam.GeometryUtility.GeometryCreationManager.RectangularBoundingBox(commonParams, rectangularParams);
                var boundingBoxChain = ChainManager.ChainGeometry(boundingBox);

                GraphicsManager.FitScreen();
                void SendLeftClick(int posX, int posY)
                {
                    Cursor.Position = new Point(posX, posY);
                    mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
                    Thread.Sleep(100);
                    mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
                }
                new System.Threading.Timer((x) => { GraphicsManager.SetFocusToGraphicsWindow(); }, null, 1000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{A}"); }, null, 2000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendLeftClick(1200, 600); }, null, 3000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{ENTER}"); }, null, 4000, Timeout.Infinite);

                var selectedCutChain = ChainManager.GetMultipleChains("");
                foreach (var chain in selectedCutChain){
                    foreach (var t in boundingBoxChain)
                    {
                        if (chain.Area != t.Area)
                        {
                            chain.Direction = ChainDirectionType.Clockwise;
                            var lowerChainLarge = chain.OffsetChain2D(OffsetSideType.Right, .0225, OffsetRollCornerType.None, .5, false, .005, false);
                            var lowerChainSmall = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometry = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometry)
                            {
                                entity.Color = 11;
                                entity.Selected = true;
                                entity.Commit();
                            }
                            GeometryManipulationManager.MoveSelectedGeometryToLevel(createdLowerLevel, true);
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));
                            GraphicsManager.Repaint(true);
                            var upperChainLarge = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var upperChainSmall = chain.OffsetChain2D(OffsetSideType.Left, .0385, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometryNew = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometryNew)
                            {
                                entity.Color = 10;
                                entity.Selected = true;
                                entity.Commit();
                            }
                            GeometryManipulationManager.MoveSelectedGeometryToLevel(createdUpperLevel, true);
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));
                            GraphicsManager.Repaint(true);
                        }
                    }

                }
                foreach (var entity in boundingBox)
                {
                    entity.Retrieve();
                    entity.Delete();
                }
                GraphicsManager.Repaint(true);
            } // Offsets cut side geometry
            void offsetCutchain81()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(81);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(81, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);


                int createdUpperLevel = 500;
                int createdLowerLevel = 501;
                LevelsManager.SetLevelName(500, "Upper Created Cut Geo");
                LevelsManager.SetLevelName(501, "Lower Created Cut Geo");
                var commonParams = new BoundingBoxCommonParams { CreateLinesArcs = true };
                var rectangularParams = new BoundingBoxRectangularParams { ExpandXMinus = 2.0, ExpandXPlus = 2.0, ExpandYMinus = 2.0, ExpandYPlus = 2.0 };
                var boundingBox = Mastercam.GeometryUtility.GeometryCreationManager.RectangularBoundingBox(commonParams, rectangularParams);
                var boundingBoxChain = ChainManager.ChainGeometry(boundingBox);

                GraphicsManager.FitScreen();
                void SendLeftClick(int posX, int posY)
                {
                    Cursor.Position = new Point(posX, posY);
                    mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
                    Thread.Sleep(100);
                    mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
                }
                new System.Threading.Timer((x) => { GraphicsManager.SetFocusToGraphicsWindow(); }, null, 1000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{A}"); }, null, 2000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendLeftClick(1200, 600); }, null, 3000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{ENTER}"); }, null, 4000, Timeout.Infinite);

                var selectedCutChain = ChainManager.GetMultipleChains("");
                var chainDirection = ChainDirectionType.Clockwise;
                foreach (var chain in selectedCutChain){
                    foreach (var t in boundingBoxChain)
                    {
                        if (chain.Area != t.Area)
                        {
                            chain.Direction = chainDirection;
                            var lowerChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0225, OffsetRollCornerType.None, .5, false, .005, false);
                            var lowerChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometry = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometry)
                            {
                                entity.Color = 11;
                                entity.Selected = true;
                                entity.Commit();
                            }
                            GeometryManipulationManager.MoveSelectedGeometryToLevel(createdLowerLevel, true);
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));
                            GraphicsManager.Repaint(true);

                            var upperChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var upperChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0385, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometryNew = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometryNew)
                            {
                                entity.Color = 10;
                                entity.Selected = true;
                                entity.Commit();
                            }
                            GeometryManipulationManager.MoveSelectedGeometryToLevel(createdUpperLevel, true);
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));
                            GraphicsManager.Repaint(true);
                        }
                    }
                }
                foreach (var entity in boundingBox)
                {
                    entity.Retrieve();
                    entity.Delete();
                }
                GraphicsManager.Repaint(true);
            } // offsets non-cut side geometry
            void shortenChains500()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(500);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(500, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(500);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);
                foreach (var chain in selectedChains){
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (firstIsFlipped == false){
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line){
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);  
                            var scale = ((lineLength-0.010)/lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (firstIsFlipped == true){
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line){
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == false){
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line){
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == true){
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line){
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            } // Shortens chains on level 500
            void shortenChains501()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(501);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(501, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(501);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);
                foreach (var chain in selectedChains)
                {
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (firstIsFlipped == false)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (firstIsFlipped == true)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint))*arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == false)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == true)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            } // Shortens chains on level 501

            offsetCutchain80();
            offsetCutchain81();
            shortenChains500();
            shortenChains501();

            return MCamReturn.NoErrors;
        }
    }
}

        
    


            

                



            

        


    


/*

void offsetCutchain();
{

    var selectedChain = ChainManager.ChainAll();
    int createdUpperLevel = 500;
    int createdLowerLevel = 501;
    LevelsManager.SetLevelName(500, "Upper Created Geo");
    LevelsManager.SetLevelName(501, "Lower Created Geo");

    foreach (var chain in selectedChain)
    {


        var lowerChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0225, OffsetRollCornerType.None, .5, false, .005, false);
        var lowerLargeGeometry = ChainManager.GetGeometryInChain(lowerChainLarge);

        var lowerChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.None, .5, false, .005, false);
        var lowerSmallGeometry = ChainManager.GetGeometryInChain(lowerChainSmall);

        var resultGeometry = SearchManager.GetResultGeometry();
        foreach (var entity in resultGeometry)
        {
            entity.Color = 11;
            entity.Selected = true;
            entity.Commit();
        }
        GeometryManipulationManager.MoveSelectedGeometryToLevel(createdLowerLevel, true);
        GraphicsManager.ClearColors(new GroupSelectionMask(true));

        var upperChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.None, .5, false, .005, false);
        var upperLargeGeometry = ChainManager.GetGeometryInChain(upperChainLarge);

        var upperChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0385, OffsetRollCornerType.None, .5, false, .005, false);
        var upperSmallGeometry = ChainManager.GetGeometryInChain(upperChainSmall);

        var resultGeometryNew = SearchManager.GetResultGeometry();
        foreach (var entity in resultGeometryNew)
        {
            entity.Color = 10;
            entity.Selected = true;
            entity.Commit();
        }
        GeometryManipulationManager.MoveSelectedGeometryToLevel(createdUpperLevel, true);
        GraphicsManager.ClearColors(new GroupSelectionMask(true));

    }


}





// Working Offset Chain
/*

var selectedChain = ChainManager.GetOneChain("Select a Chain");

var offsetChain = selectedChain.OffsetChain2D(OffsetSideType.Left,
                                              .245,
                                              OffsetRollCornerType.None,
                                              .5,
                                              false,
                                              .005,
                                              false);

var offsetGeometry = ChainManager.GetGeometryInChain(offsetChain);

foreach (var entity in offsetGeometry)
{
    entity.Commit();
}

return MCamReturn.NoErrors;
*/





//Working Translate
/*
bool MoveLine() {
    bool result = false;
    //Mastercam.IO.SelectionManager.SelectAllGeometry();
    Mastercam.Math.Point3D pt1 = new Mastercam.Math.Point3D(0.0, 0.0, 0.0);
    Mastercam.Math.Point3D pt2 = new Mastercam.Math.Point3D(100.0, 0.0, 0.0);
    MCView Top = new MCView();
    Mastercam.GeometryUtility.GeometryManipulationManager.TranslateGeometry(pt1, pt2, Top , Top, false);
    return result;
}
MoveLine();
*/


// working form
/*
var m = new Form1();
m.Show();
*/

// working line creation
/*
bool CreateLine()
{
    bool result = false;

    Mastercam.Math.Point3D pt1 = new Mastercam.Math.Point3D(0.0, 0.0, 0.0);
    Mastercam.Math.Point3D pt2 = new Mastercam.Math.Point3D(100.0, 0.0, 0.0);
    Mastercam.Curves.LineGeometry Line1 = new Mastercam.Curves.LineGeometry(pt1, pt2);
    result = Line1.Commit();
    result = Line1.Validate(); // Not really needed here, if Commit was successful - we're good!
                               //Mastercam.IO.GraphicsManager.Repaint(True)

    return result;
}
CreateLine();
*/

//working popup message
//  System.Windows.Forms.MessageBox.Show("Jeremy can make pop up messages!");
//return Mastercam.App.Types.MCamReturn.NoErrors;
