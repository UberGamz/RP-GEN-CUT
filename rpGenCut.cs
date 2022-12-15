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
using System.Linq;
using Mastercam.BasicGeometry;
using Mastercam.IO.Types;
using System.Xml;

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
            var templist500 = new List<int>();
            var anotherTemplist500 = new List<int>();
            var templist501 = new List<int>();
            var anotherTemplist501 = new List<int>();
            var pointList = new List<int>();
            var lineList = new List<int>();
            var secondPointList = new List<int>();
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
                        }
                    }

                }
                foreach (var entity in boundingBox)
                {
                    entity.Retrieve();
                    entity.Delete();
                }
            } // Offsets cut side geometry
            void offsetCutchain81()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(81);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown){
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(81, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                GraphicsManager.Repaint();
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
                        }
                    }
                }
                foreach (var entity in boundingBox)
                {
                    entity.Retrieve();
                    entity.Delete();
                }
            } // offsets non-cut side geometry
            void shortenChains500()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(500);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown){
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(500, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint();
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
            } // Shortens chains on level 500
            void shortenChains501()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(501);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown){
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(501, true);
                LevelsManager.RefreshLevelsManager();
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
            } // Shortens chains on level 501
            void findArcChainEnds500()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(500);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown){
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
                    if (Geometry.RetrieveEntity(firstEntity) is ArcGeometry startArc){
                        startArc.Color = 70;
                        startArc.Commit();
                        templist500.Add(firstEntity);
                        anotherTemplist500.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is ArcGeometry endArc){
                        endArc.Color = 70;
                        endArc.Commit();
                        templist500.Add(lastEntity);
                        anotherTemplist500.Add(lastEntity);
                    }
                }
                var tempListArc = new List<int>();
                var tempListRad = new List<double>();
                var rad1 = 0;
                var rad2 = 0;
                var rad3 = 0;
                var rad4 = 0;
                foreach (var arc in templist500){
                    if (Geometry.RetrieveEntity(arc) is ArcGeometry firstArc && firstArc.Color != 60){
                        tempListArc.Add(arc);
                        tempListRad.Add(firstArc.Data.Radius);
                        var firstArcCP = firstArc.Data.CenterPoint;
                        foreach (var anotherArc in templist500){
                            if (Geometry.RetrieveEntity(anotherArc) is ArcGeometry nextArc && anotherArc != arc && nextArc.Color != 60){
                                var nextArcCP = nextArc.Data.CenterPoint;
                                if (VectorManager.Distance(firstArcCP, nextArcCP) < 0.001){
                                    tempListRad.Add(nextArc.Data.Radius);
                                    tempListArc.Add(anotherArc);
                                    if (tempListArc.Count == 4){
                                        foreach (var thisArc in tempListArc){
                                            var entity = Geometry.RetrieveEntity(thisArc);
                                            entity.Color = 60;
                                            entity.Commit();
                                            tempListRad.Sort();
                                        }
                                        foreach (var i in tempListArc){
                                            var thisEntity = Geometry.RetrieveEntity(i);
                                            if (thisEntity is ArcGeometry rad){
                                                if (rad.Data.Radius == tempListRad[0]){
                                                    rad1 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[1]){
                                                    rad2 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[2]){
                                                    rad3 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[3]){
                                                    rad4 = (thisEntity.GetEntityID());
                                                }
                                            }
                                        }
                                        var newRad1 = Geometry.RetrieveEntity(rad1);
                                        var newRad2 = Geometry.RetrieveEntity(rad2);
                                        var newRad3 = Geometry.RetrieveEntity(rad3);
                                        var newRad4 = Geometry.RetrieveEntity(rad4);
                                        var arc1End = new Point3D();
                                        var arc2End = new Point3D();
                                        var arc3End = new Point3D();
                                        var arc4End = new Point3D();
                                        newRad1.Commit();
                                        newRad2.Commit();
                                        newRad3.Commit();
                                        newRad4.Commit();

                                        if (newRad2 is ArcGeometry arc2)
                                        {
                                            var arcStartPoint = (arc2.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc2.Data.EndAngleDegrees);
                                            var arcRad = arc2.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc2.Data.StartAngleDegrees + 360) > (arc2.Data.EndAngleDegrees + 360) && (arc2.Data.StartAngleDegrees >= 90) && (arc2.Data.StartAngleDegrees <= 260))
                                            {
                                                arc2.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint2.x, arc2.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                            else
                                            {
                                                arc2.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint1.x, arc2.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                        }
                                        if (newRad3 is ArcGeometry arc3)
                                        {
                                            var arcStartPoint = (arc3.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc3.Data.EndAngleDegrees);
                                            var arcRad = arc3.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc3.Data.StartAngleDegrees + 360) > (arc3.Data.EndAngleDegrees + 360) && (arc3.Data.StartAngleDegrees >= 90) && (arc3.Data.StartAngleDegrees <= 260))
                                            {
                                                arc3.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint1.x, arc3.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                            else
                                            {
                                                arc3.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint2.x, arc3.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                        }
                                        if (newRad1 is ArcGeometry arc1)
                                        {
                                            if ((arc1.Data.StartAngleDegrees + 360) > (arc1.Data.EndAngleDegrees + 360) && (arc1.Data.StartAngleDegrees >= 90) && (arc1.Data.StartAngleDegrees <= 260))
                                            {
                                                arc1End = new Point3D(arc1.EndPoint1.x, arc1.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                            else
                                            {
                                                arc1End = new Point3D(arc1.EndPoint2.x, arc1.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                        }
                                        if (newRad4 is ArcGeometry arc4)
                                        {
                                            if ((arc4.Data.StartAngleDegrees + 360) > (arc4.Data.EndAngleDegrees + 360) && (arc4.Data.StartAngleDegrees >= 90) && (arc4.Data.StartAngleDegrees <= 260))
                                            {
                                                arc4End = new Point3D(arc4.EndPoint2.x, arc4.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                            else
                                            {
                                                arc4End = new Point3D(arc4.EndPoint1.x, arc4.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                        }

                                        var line1 = new LineGeometry(arc1End, arc2End);
                                        var line2 = new LineGeometry(arc3End, arc4End);
                                        line1.Color = 10;
                                        line2.Color = 10;
                                        line1.Commit();
                                        line2.Commit();
                                        tempListArc.Clear();
                                        tempListRad.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var chain in ChainManager.ChainAll(500)){
                    var chainGeo = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeo){
                        entity.Color = 70;
                        entity.Selected = false;
                        entity.Commit();
                    }
                }
                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            }
            void findArcChainEnds501()
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
                    if (Geometry.RetrieveEntity(firstEntity) is ArcGeometry startArc)
                    {
                        startArc.Color = 70;
                        startArc.Commit();
                        templist501.Add(firstEntity);
                        anotherTemplist501.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is ArcGeometry endArc)
                    {
                        endArc.Color = 70;
                        endArc.Commit();
                        templist501.Add(lastEntity);
                        anotherTemplist501.Add(lastEntity);
                    }
                }
                var tempListArc = new List<int>();
                var tempListRad = new List<double>();
                var rad1 = 0;
                var rad2 = 0;
                var rad3 = 0;
                var rad4 = 0;
                foreach (var arc in templist501){
                    if (Geometry.RetrieveEntity(arc) is ArcGeometry firstArc && firstArc.Color != 60){
                        tempListArc.Add(arc);
                        tempListRad.Add(firstArc.Data.Radius);
                        var firstArcCP = firstArc.Data.CenterPoint;
                        foreach (var anotherArc in templist501){
                            if (Geometry.RetrieveEntity(anotherArc) is ArcGeometry nextArc && anotherArc != arc && nextArc.Color != 60){
                                var nextArcCP = nextArc.Data.CenterPoint;
                                if (VectorManager.Distance(firstArcCP, nextArcCP) < 0.001){
                                    tempListRad.Add(nextArc.Data.Radius);
                                    tempListArc.Add(anotherArc);
                                    if (tempListArc.Count == 4){
                                        foreach (var thisArc in tempListArc){
                                            var entity = Geometry.RetrieveEntity(thisArc);
                                            entity.Color = 60;
                                            entity.Commit();
                                            tempListRad.Sort();
                                        }
                                        foreach (var i in tempListArc){
                                            var thisEntity = Geometry.RetrieveEntity(i);
                                            if (thisEntity is ArcGeometry rad){
                                                if (rad.Data.Radius == tempListRad[0]){
                                                    rad1 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[1]){
                                                    rad2 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[2]){
                                                    rad3 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[3]){
                                                    rad4 = (thisEntity.GetEntityID());
                                                }
                                            }
                                        }
                                        var newRad1 = Geometry.RetrieveEntity(rad1);
                                        var newRad2 = Geometry.RetrieveEntity(rad2);
                                        var newRad3 = Geometry.RetrieveEntity(rad3);
                                        var newRad4 = Geometry.RetrieveEntity(rad4);
                                        var arc1End = new Point3D();
                                        var arc2End = new Point3D();
                                        var arc3End = new Point3D();
                                        var arc4End = new Point3D();
                                        newRad1.Commit();
                                        newRad2.Commit();
                                        newRad3.Commit();
                                        newRad4.Commit();

                                        if (newRad2 is ArcGeometry arc2)
                                        {
                                            var arcStartPoint = (arc2.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc2.Data.EndAngleDegrees);
                                            var arcRad = arc2.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc2.Data.StartAngleDegrees + 360) > (arc2.Data.EndAngleDegrees + 360) && (arc2.Data.StartAngleDegrees >= 90) && (arc2.Data.StartAngleDegrees <= 260))
                                            {
                                                arc2.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint1.x, arc2.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                            else
                                            {
                                                arc2.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint2.x, arc2.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                        }
                                        if (newRad3 is ArcGeometry arc3)
                                        {
                                            var arcStartPoint = (arc3.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc3.Data.EndAngleDegrees);
                                            var arcRad = arc3.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc3.Data.StartAngleDegrees + 360) > (arc3.Data.EndAngleDegrees + 360) && (arc3.Data.StartAngleDegrees >= 90) && (arc3.Data.StartAngleDegrees <= 260))
                                            {
                                                arc3.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint2.x, arc3.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                            else
                                            {
                                                arc3.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint1.x, arc3.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                        }
                                        if (newRad1 is ArcGeometry arc1)
                                        {
                                            if ((arc1.Data.StartAngleDegrees + 360) > (arc1.Data.EndAngleDegrees + 360) && (arc1.Data.StartAngleDegrees >= 90) && (arc1.Data.StartAngleDegrees <= 260))
                                            {
                                                arc1End = new Point3D(arc1.EndPoint2.x, arc1.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                            else
                                            {
                                                arc1End = new Point3D(arc1.EndPoint1.x, arc1.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                        }
                                        if (newRad4 is ArcGeometry arc4)
                                        {
                                            if ((arc4.Data.StartAngleDegrees + 360) > (arc4.Data.EndAngleDegrees + 360) && (arc4.Data.StartAngleDegrees >= 90) && (arc4.Data.StartAngleDegrees <= 260))
                                            {
                                                arc4End = new Point3D(arc4.EndPoint1.x, arc4.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                            else
                                            {
                                                arc4End = new Point3D(arc4.EndPoint2.x, arc4.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                        }

                                        var line1 = new LineGeometry(arc1End, arc2End);
                                        var line2 = new LineGeometry(arc3End, arc4End);
                                        line1.Color = 10;
                                        line2.Color = 10;
                                        line1.Commit();
                                        line2.Commit();
                                        tempListArc.Clear();
                                        tempListRad.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                
                foreach (var chain in ChainManager.ChainAll(501)){
                    var chainGeo = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeo){
                        entity.Color = 70;
                        entity.Selected = false;
                        entity.Commit();
                    }
                }
                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            }
            void findLineChainEnds500()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(500);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown){LevelsManager.SetLevelVisible(level, false);}
                LevelsManager.SetLevelVisible(500, true);
                LevelsManager.SetLevelVisible(50, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                SelectionManager.SelectGeometryByMask(Mastercam.IO.Types.QuickMaskType.Points);
                var selectedGeometry = SearchManager.GetSelectedGeometry();
                foreach (var point in selectedGeometry){pointList.Add(point.GetEntityID());}
                LevelsManager.SetLevelVisible(50, false);
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
                    if (Geometry.RetrieveEntity(firstEntity) is LineGeometry startLine){
                        startLine.Color = 70;
                        startLine.Commit();
                        templist500.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is LineGeometry endLine){
                        endLine.Color = 70;
                        endLine.Commit();
                        templist500.Add(lastEntity);
                    }
                }
                foreach (var line in templist500) {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine){
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) {result = (result + 360);}
                        if (result >=0 && result <= 89){
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 90 && result <= 179){
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 180 && result <= 269){
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 270 && result <= 359){
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                    }
                }
                foreach (var point in secondPointList) {
                    var step = 0;
                    var newLine = new LineGeometry();
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    if (thisPoint is PointGeometry yesThisPoint){
                        var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                        foreach (var centerPoint in pointList){
                            var thisCenterPoint = Geometry.RetrieveEntity(centerPoint);
                            if (thisCenterPoint is PointGeometry yesThisCenterPoint){
                                var finallyThisCenterPoint = new Point3D(yesThisCenterPoint.Data.x, yesThisCenterPoint.Data.y, 0.0);
                                if (step == 0) { 
                                    newLine = new LineGeometry(finallyThisPoint, finallyThisCenterPoint);
                                    newLine.Commit();
                                    step = 1;
                                }
                                if (VectorManager.Distance(finallyThisPoint, finallyThisCenterPoint) < VectorManager.Distance(newLine.Data.Point1, newLine.Data.Point2) && step == 1){
                                    newLine.Data.Point2 = finallyThisCenterPoint;
                                    newLine.Color = 90;
                                    newLine.Commit();
                                }
                                lineList.Add(newLine.GetEntityID());
                            }
                        }
                    }
                }
                foreach (var point in pointList) {
                    var tempLineListDown = new List<double>();
                    var tempLinesGoingDown = new List<int>();
                    var tempLineListUp = new List<double>();
                    var tempLinesGoingUp = new List<int>();
                    if (Geometry.RetrieveEntity(point) is PointGeometry centerPoint){
                        var pointGeo = new Point3D(centerPoint.Data.x, centerPoint.Data.y, 0.0);
                        foreach (var entity in lineList){
                            if (Geometry.RetrieveEntity(entity) is LineGeometry line){
                                if(VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001){
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 0 && result <= 179){
                                        var thisLine = line.GetEntityID();
                                        if(tempLinesGoingDown.Contains(thisLine) == false) { tempLinesGoingDown.Add(thisLine); }
                                        if (tempLineListDown.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListDown.Add(VectorManager.Distance(pt1, pt2));}
                                        if (tempLineListDown.Count == 4) {
                                            tempLineListDown.Sort();
                                            foreach (var extraLine in tempLinesGoingDown){
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i){
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListDown[0]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[1]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[2]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[3]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingDown.Clear();
                                            tempLineListDown.Clear();
                                        }
                                    }
                                }
                                if (VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001){
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 180 && result <= 259){
                                        var thisLine = line.GetEntityID();
                                        if (tempLinesGoingUp.Contains(thisLine) == false) { tempLinesGoingUp.Add(thisLine); }
                                        if (tempLineListUp.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListUp.Add(VectorManager.Distance(pt1, pt2)); }
                                        if (tempLineListUp.Count == 4){
                                            tempLineListUp.Sort();
                                            foreach (var extraLine in tempLinesGoingUp){
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i){
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListUp[0]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[1]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[2]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[3]){
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList){
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint){
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001){
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingUp.Clear();
                                            tempLineListUp.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist500){
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine){
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        foreach (var endPoint in secondPointList){
                            if (Geometry.RetrieveEntity(endPoint) is PointGeometry pointy){
                                var point = new Point3D(pointy.Data.x, pointy.Data.y, 0.0);
                                if (VectorManager.Distance(pt1, point) <= 0.001 || (VectorManager.Distance(pt2, point) <= 0.001)){
                                    noThisLine.Color = pointy.Color;
                                    noThisLine.Commit();
                                    pointy.Delete();
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist500){
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine){
                        var pt1 = noThisLine.EndPoint1;
                        var pt2 = noThisLine.EndPoint2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) { result = (result + 360); }
                        if (result >= 0 && result <= 89){
                            if (thisLine.Color == 50){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                        if (result >= 90 && result <= 179){
                            if (thisLine.Color == 50){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                        if (result >= 180 && result <= 269){
                            if (thisLine.Color == 50){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                        if (result >= 270 && result <= 359){
                            if (thisLine.Color == 50){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53){
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                    }
                }





                        /*
                        foreach (var chain in ChainManager.ChainAll(500)){
                            var chainGeo = ChainManager.GetGeometryInChain(chain);
                            foreach (var entity in chainGeo)
                            {
                                entity.Color = 70;
                                entity.Selected = false;
                                entity.Commit();
                            }
                        }
                        */
                        SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            }
            void findLineChainEnds501()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(501);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown) { LevelsManager.SetLevelVisible(level, false); }
                LevelsManager.SetLevelVisible(501, true);
                LevelsManager.SetLevelVisible(50, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                SelectionManager.SelectGeometryByMask(Mastercam.IO.Types.QuickMaskType.Points);
                var selectedGeometry = SearchManager.GetSelectedGeometry();
                foreach (var point in selectedGeometry) { pointList.Add(point.GetEntityID()); }
                LevelsManager.SetLevelVisible(50, false);
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
                    if (Geometry.RetrieveEntity(firstEntity) is LineGeometry startLine)
                    {
                        startLine.Color = 70;
                        startLine.Commit();
                        templist501.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is LineGeometry endLine)
                    {
                        endLine.Color = 70;
                        endLine.Commit();
                        templist501.Add(lastEntity);
                    }
                }
                foreach (var line in templist501)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) { result = (result + 360); }
                        if (result >= 0 && result <= 89)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 90 && result <= 179)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 180 && result <= 269)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 270 && result <= 359)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                    }
                }
                foreach (var point in secondPointList)
                {
                    var step = 0;
                    var newLine = new LineGeometry();
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    if (thisPoint is PointGeometry yesThisPoint)
                    {
                        var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                        foreach (var centerPoint in pointList)
                        {
                            var thisCenterPoint = Geometry.RetrieveEntity(centerPoint);
                            if (thisCenterPoint is PointGeometry yesThisCenterPoint)
                            {
                                var finallyThisCenterPoint = new Point3D(yesThisCenterPoint.Data.x, yesThisCenterPoint.Data.y, 0.0);
                                if (step == 0)
                                {
                                    newLine = new LineGeometry(finallyThisPoint, finallyThisCenterPoint);
                                    newLine.Commit();
                                    step = 1;
                                }
                                if (VectorManager.Distance(finallyThisPoint, finallyThisCenterPoint) < VectorManager.Distance(newLine.Data.Point1, newLine.Data.Point2) && step == 1)
                                {
                                    newLine.Data.Point2 = finallyThisCenterPoint;
                                    newLine.Color = 90;
                                    newLine.Commit();
                                }
                                lineList.Add(newLine.GetEntityID());
                            }
                        }
                    }
                }
                foreach (var point in pointList)
                {
                    var tempLineListDown = new List<double>();
                    var tempLinesGoingDown = new List<int>();
                    var tempLineListUp = new List<double>();
                    var tempLinesGoingUp = new List<int>();
                    if (Geometry.RetrieveEntity(point) is PointGeometry centerPoint)
                    {
                        var pointGeo = new Point3D(centerPoint.Data.x, centerPoint.Data.y, 0.0);
                        foreach (var entity in lineList)
                        {
                            if (Geometry.RetrieveEntity(entity) is LineGeometry line)
                            {
                                if (VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001)
                                {
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 0 && result <= 179)
                                    {
                                        var thisLine = line.GetEntityID();
                                        if (tempLinesGoingDown.Contains(thisLine) == false) { tempLinesGoingDown.Add(thisLine); }
                                        if (tempLineListDown.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListDown.Add(VectorManager.Distance(pt1, pt2)); }
                                        if (tempLineListDown.Count == 4)
                                        {
                                            tempLineListDown.Sort();
                                            foreach (var extraLine in tempLinesGoingDown)
                                            {
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i)
                                                {
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListDown[0])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[1])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[2])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[3])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingDown.Clear();
                                            tempLineListDown.Clear();
                                        }
                                    }
                                }
                                if (VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001)
                                {
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 180 && result <= 259)
                                    {
                                        var thisLine = line.GetEntityID();
                                        if (tempLinesGoingUp.Contains(thisLine) == false) { tempLinesGoingUp.Add(thisLine); }
                                        if (tempLineListUp.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListUp.Add(VectorManager.Distance(pt1, pt2)); }
                                        if (tempLineListUp.Count == 4)
                                        {
                                            tempLineListUp.Sort();
                                            foreach (var extraLine in tempLinesGoingUp)
                                            {
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i)
                                                {
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListUp[0])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[1])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[2])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[3])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingUp.Clear();
                                            tempLineListUp.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist501)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        foreach (var endPoint in secondPointList)
                        {
                            if (Geometry.RetrieveEntity(endPoint) is PointGeometry pointy)
                            {
                                var point = new Point3D(pointy.Data.x, pointy.Data.y, 0.0);
                                if (VectorManager.Distance(pt1, point) <= 0.001 || (VectorManager.Distance(pt2, point) <= 0.001))
                                {
                                    noThisLine.Color = pointy.Color;
                                    noThisLine.Commit();
                                    pointy.Delete();
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist501)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.EndPoint1;
                        var pt2 = noThisLine.EndPoint2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) { result = (result + 360); }
                        if (result >= 0 && result <= 89)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                        if (result >= 90 && result <= 179)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                        if (result >= 180 && result <= 269)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                        if (result >= 270 && result <= 359)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Commit();
                            }
                        }
                    }
                }





                
                foreach (var chain in ChainManager.ChainAll(501)){
                    var chainGeo = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeo)
                    {
                        entity.Color = 70;
                        entity.Selected = false;
                        entity.Commit();
                    }
                }
                
                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            }




            //offsetCutchain80();
            //offsetCutchain81();
            //shortenChains500();
            //shortenChains501();
            //findArcChainEnds500();
            //findArcChainEnds501();
            findLineChainEnds500();
            findLineChainEnds501();



            return MCamReturn.NoErrors;
        }
    }
}
