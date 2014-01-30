using FlatRedBall.Math;
using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatRedBall.TileCollisions
{
    public class TileShapeCollection
    {
        #region Fields

        ShapeCollection mShapes;
        Axis mSortAxis = Axis.X;
        float mLeftSeedX = 0;
        float mBottomSeedY = 0;
        float mGridSize;
        bool mVisible = true;

        #endregion

        #region Properties

        public Axis SortAxis
        {
            get
            {
                return mSortAxis;
            }
            set
            {
                bool hasChanged = value != mSortAxis;
                if (hasChanged)
                {
                    mSortAxis = value;
                    PerformSort();
                }
            }
        }

        public float GridSize
        {
            get { return mGridSize; }
            set
            {
                mGridSize = value;
                mShapes.MaxAxisAlignedRectanglesScale = mGridSize;
            }
        }

        PositionedObjectList<AxisAlignedRectangle> Rectangles
        {
            get { return mShapes.AxisAlignedRectangles; }
        }

        public bool Visible
        {
            get { return mVisible; }
            set
            {
                mVisible = value;
                for (int i = 0; i < mShapes.AxisAlignedRectangles.Count; i++)
                {
                    mShapes.AxisAlignedRectangles[i].Visible = value;
                }
            }
        }

        #endregion

        public TileShapeCollection()
        {
            mShapes = new ShapeCollection();
            GridSize = 16;
        }

        public bool CollideAgainstSolid(AxisAlignedRectangle movableObject)
        {
            bool toReturn = false;




            toReturn = mShapes.CollideAgainstBounce(movableObject, true, mSortAxis, 1, 0, 0);

            return toReturn;
        }


        public AxisAlignedRectangle GetTileAt(float x, float y)
        {
            float roundedX = MathFunctions.RoundFloat(x, GridSize, mLeftSeedX + GridSize/2.0f);
            float roundedY = MathFunctions.RoundFloat(y, GridSize, mBottomSeedY + GridSize/2.0f);
            float keyValue = GetKeyValue(roundedX, roundedY);

            float keyValueBefore = keyValue - GridSize / 2.0f;
            float keyValueAfter = keyValue + GridSize / 2.0f;

            int startInclusive = mShapes.AxisAlignedRectangles.GetFirstAfter(keyValueBefore, mSortAxis,
                0, mShapes.AxisAlignedRectangles.Count);


            int endExclusive = mShapes.AxisAlignedRectangles.GetFirstAfter(keyValueAfter, mSortAxis,
                0, mShapes.AxisAlignedRectangles.Count);

            AxisAlignedRectangle toReturn = GetTileAt(x, y, startInclusive, endExclusive);

            return toReturn;
        }

        private AxisAlignedRectangle GetTileAt(float x, float y, int startInclusive, int endExclusive)
        {
            AxisAlignedRectangle toReturn = null;
            for (int i = startInclusive; i < endExclusive; i++)
            {
                if (mShapes.AxisAlignedRectangles[i].IsPointInside(x, y))
                {
                    toReturn = mShapes.AxisAlignedRectangles[i];
                    break;
                }
            }
            return toReturn;
        }

        public void AddCollisionAtWorld(float x, float y)
        {
            // Make sure there isn't already collision here
            if (GetTileAt(x, y) == null)
            {
                // x and y
                // represent
                // the center
                // of the tile
                // where the user
                // may want to add 
                // collision.  Let's
                // subtract half width/
                // height so we can use the
                // bottom/left
                float roundedX = MathFunctions.RoundFloat(x - GridSize / 2.0f, GridSize, mLeftSeedX);
                float roundedY = MathFunctions.RoundFloat(y - GridSize / 2.0f, GridSize, mBottomSeedY);

                AxisAlignedRectangle newAar = new AxisAlignedRectangle();
                newAar.Width = GridSize;
                newAar.Height = GridSize;
                newAar.Left = roundedX;
                newAar.Bottom = roundedY;

                if (this.mVisible)
                {
                    newAar.Visible = true;
                }

                float keyValue = GetKeyValue(roundedX, roundedY);

                int index = mShapes.AxisAlignedRectangles.GetFirstAfter(keyValue, mSortAxis,
                    0, mShapes.AxisAlignedRectangles.Count);

                mShapes.AxisAlignedRectangles.Insert(index, newAar);

                UpdateRepositionDirectionsFor(newAar);
            }
        }

        public void RemoveCollisionAtWorld(float x, float y)
        {
            AxisAlignedRectangle existing = GetTileAt(x, y);
            if(existing != null)
            {
                ShapeManager.Remove(existing);

                float keyValue = GetKeyValue(existing.X, existing.Y);

                float keyValueBefore = keyValue - GridSize * 3 / 2.0f;
                float keyValueAfter = keyValue + GridSize * 3 / 2.0f;

                int before = Rectangles.GetFirstAfter(keyValueBefore, mSortAxis, 0, Rectangles.Count);
                int after = Rectangles.GetFirstAfter(keyValueAfter, mSortAxis, 0, Rectangles.Count);

                AxisAlignedRectangle leftOf = GetTileAt(existing.X - GridSize, existing.Y, before, after);
                AxisAlignedRectangle rightOf = GetTileAt(existing.X + GridSize, existing.Y, before, after);
                AxisAlignedRectangle above = GetTileAt(existing.X, existing.Y + GridSize, before, after);
                AxisAlignedRectangle below = GetTileAt(existing.X, existing.Y - GridSize, before, after);

                if (leftOf != null && (leftOf.RepositionDirections & RepositionDirections.Right) != RepositionDirections.Right)
                {
                    leftOf.RepositionDirections |= RepositionDirections.Right;

                }
                if (rightOf != null && (rightOf.RepositionDirections & RepositionDirections.Left) != RepositionDirections.Left)
                {
                    rightOf.RepositionDirections |= RepositionDirections.Left;
                }

                if (above != null && (above.RepositionDirections & RepositionDirections.Down) != RepositionDirections.Down)
                {
                    above.RepositionDirections |= RepositionDirections.Down;
                }

                if (below != null && (below.RepositionDirections & RepositionDirections.Up) != RepositionDirections.Up)
                {
                    below.RepositionDirections |= RepositionDirections.Up;
                }
                

            }


        }

        private float GetKeyValue(float x, float y)
        {
            float keyValue = 0;

            switch (mSortAxis)
            {
                case Axis.X:
                    keyValue = x;
                    break;
                case Axis.Y:
                    keyValue = y;
                    break;
                case Axis.Z:
                    throw new NotImplementedException("Sorting on Z not supported");
            }
            return keyValue;
        }

        private void UpdateRepositionDirectionsFor(AxisAlignedRectangle newAar)
        {
            // Let's see what is surrounding this rectangle and update it and the surrounding rects appropriately
            float keyValue = GetKeyValue(newAar.X, newAar.Y);

            float keyValueBefore = keyValue - GridSize * 3 / 2.0f;
            float keyValueAfter = keyValue + GridSize * 3 / 2.0f;

            int before = Rectangles.GetFirstAfter(keyValueBefore, mSortAxis, 0, Rectangles.Count);
            int after = Rectangles.GetFirstAfter(keyValueAfter, mSortAxis, 0, Rectangles.Count);

            AxisAlignedRectangle leftOf = GetTileAt(newAar.X - GridSize, newAar.Y, before, after);
            AxisAlignedRectangle rightOf = GetTileAt(newAar.X + GridSize, newAar.Y, before, after);
            AxisAlignedRectangle above = GetTileAt(newAar.X, newAar.Y + GridSize, before, after);
            AxisAlignedRectangle below = GetTileAt(newAar.X, newAar.Y - GridSize, before, after);

            RepositionDirections directions = RepositionDirections.All;
            if (leftOf != null)
            {
                directions -= RepositionDirections.Left;
                if ((leftOf.RepositionDirections & RepositionDirections.Right) == RepositionDirections.Right)
                {
                    leftOf.RepositionDirections -= RepositionDirections.Right;
                }
            }
            if (rightOf != null)
            {
                directions -= RepositionDirections.Right;

                if ((rightOf.RepositionDirections & RepositionDirections.Left) == RepositionDirections.Left)
                {
                    rightOf.RepositionDirections -= RepositionDirections.Left;
                }
            }
            if (above != null)
            {
                directions -= RepositionDirections.Up;

                if ((above.RepositionDirections & RepositionDirections.Down) == RepositionDirections.Down)
                {
                    above.RepositionDirections -= RepositionDirections.Down;
                }
            }
            if (below != null)
            {
                directions -= RepositionDirections.Down;
                if ((below.RepositionDirections & RepositionDirections.Up) == RepositionDirections.Up)
                {
                    below.RepositionDirections -= RepositionDirections.Up;
                }
            }

            newAar.RepositionDirections = directions;

        }

        public void RemoveFromManagers()
        {
            this.mShapes.RemoveFromManagers();
        }

        private void PerformSort()
        {
            switch (mSortAxis)
            {
                case Axis.X:
                    mShapes.AxisAlignedRectangles.SortXInsertionAscending();
                    break;
                case Axis.Y:
                    mShapes.AxisAlignedRectangles.SortYInsertionAscending();
                    break;
                case Axis.Z:
                    mShapes.AxisAlignedRectangles.SortZInsertionAscending();
                    break;
            }
        }
    }
}
