using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /** A class for drawing a sector gizmo
     * in the editor window */
    public class SectorGizmo
    {
        /** The color of the gizmo */
        public Color color;

        /** The position of the sector */
        public Vector3 position;

        /** The rotation of the sector */
        public Quaternion rotation;

        /** The radius of the sector */
        public float radius = 1f;

        /** The angle of the sector */
        protected float angle = 90f;

        /** Number of triangles in the sector mesh */
        protected int divisions = 8;

        /** Mesh used for displaying the sector */
        protected Mesh mesh;

        /** This must be calle inside OnDrawGizmos in a MonoBehaviour class */
        public virtual void OnDrawGizmos()
        {
            Configure(angle, divisions);
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, position, rotation, Vector3.one * radius);
        }

        /** Configure some parameters of the sector gizmo
         * The mesh will be recreated if necessary.
         * @param angle The angle of the sector.
         * @param divisions Number of triangles in the sector mesh.
         */
        public virtual void Configure(float angle, int divisions)
        {
            if (mesh == null || angle != this.angle ||
                divisions != this.divisions)
            {
                this.angle = angle;
                this.divisions = divisions;
                mesh = MeshToolkit.MeshUtil.CreateSectorMesh(1, this.angle, this.divisions);
            }
        }
    }
}

