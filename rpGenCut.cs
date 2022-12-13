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
            void findChainEnds500()
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
                GraphicsManager.Repaint(true);
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
                                            GraphicsManager.Repaint(true);
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

                                        if (newRad2 is ArcGeometry arc2){
                                            var arcStartPoint = (arc2.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc2.Data.EndAngleDegrees);
                                            var arcRad = arc2.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            arc2.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                            newRad2.Commit();
                                            var cx = arc2.Data.CenterPoint.x;
                                            var cy = arc2.Data.CenterPoint.y;
                                            var x = cx + arcRad * Math.Sin(arc2.Data.StartAngleDegrees);
                                            var y = cy + arcRad * Math.Cos(arc2.Data.StartAngleDegrees);
                                            arc2End = new Point3D(x ,y ,0.0);
                                            var point = new PointGeometry(arc2End);
                                            point.Color = 11;
                                            point.Commit();
                                            newRad2.Commit();
                                        }
                                        if (newRad3 is ArcGeometry arc3){
                                            var arcStartPoint = (arc3.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc3.Data.EndAngleDegrees);
                                            var arcRad = arc3.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            arc3.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                            newRad3.Commit();
                                            var cx = arc3.Data.CenterPoint.x;
                                            var cy = arc3.Data.CenterPoint.y;
                                            var x = cx + arcRad * Math.Sin(arc3.Data.EndAngleDegrees);
                                            var y = cy + arcRad * Math.Cos(arc3.Data.EndAngleDegrees);
                                            arc3End = new Point3D(x, y, 0.0);
                                            var point = new PointGeometry(arc3End);
                                            point.Color = 12;
                                            point.Commit();
                                            newRad3.Commit();
                                        }
                                        if (newRad1 is ArcGeometry arc1){
                                            if (((arc1.Data.StartAngleDegrees + 360) - (arc1.Data.EndAngleDegrees + 360)) < arc1.Data.StartAngleDegrees)
                                            {
                                                arc1End = new Point3D(arc1.EndPoint1.x, arc1.EndPoint1.y,0.0);
                                                var point = new PointGeometry(arc1End);
                                                point.Color = 10;
                                                point.Commit();
                                            }
                                            if (((arc1.Data.StartAngleDegrees + 360) - (arc1.Data.EndAngleDegrees + 360)) > arc1.Data.StartAngleDegrees)
                                            {
                                                arc1End = new Point3D(arc1.EndPoint2.x, arc1.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                                point.Color = 10;
                                                point.Commit();
                                            }
                                        }
                                        if (newRad4 is ArcGeometry arc4){
                                            var cx = arc4.Data.CenterPoint.x;
                                            var cy = arc4.Data.CenterPoint.y;
                                            var arcRad = arc4.Data.Radius;
                                            var x = cx + arcRad * Math.Sin(arc4.Data.EndAngleDegrees);
                                            var y = cy + arcRad * Math.Cos(arc4.Data.EndAngleDegrees);
                                            arc4End = new Point3D(x, y, 0.0);
                                            var point = new PointGeometry(arc4End);
                                            point.Color = 13;
                                            point.Commit();
                                            newRad3.Commit();
                                        }

                                        //var line1 = new LineGeometry(arc1End, arc2End);
                                        //var line2 = new LineGeometry(arc3End, arc4End);
                                        //line1.Color = 10;
                                        //line2.Color = 10;
                                        //line1.Commit();
                                        //line2.Commit();
                                        tempListArc.Clear();
                                        tempListRad.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            } // Shortens chains on level 500


            //offsetCutchain80();
            //offsetCutchain81();
            //shortenChains500();
            //shortenChains501();
            findChainEnds500();

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
