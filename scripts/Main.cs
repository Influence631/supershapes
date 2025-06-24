using Godot;
using System;
using static System.Math;

public partial class Main : Node3D
{
	Vector3[,] points;
	[Export] int total = 50;
	float radius = 100f;
	Vector2 coeff1 = new Vector2(1, 1);
	Vector2 coeff2 = new Vector2(1, 1);
	const float angleRotation = 0.7f;
	Node3D shapes;
	Vector4 parameters1 = new(1,1,1,1);
	Vector4 parameters2 = new(1,1,1,1);

	public override void _Ready(){
		shapes = GetNode<Node3D>("shapes");
		GenerateFigure(radius, total, parameters1, parameters2);
	}

	public void GenerateFigure(float radius, int total, Vector4 parameters1, Vector4 parameters2){
		points = GenerateSuperPoints(coeff1, coeff2, radius, total, parameters1, parameters2);
		// points = GeneratePoints(radius, total);
		//CreateLinedShape(points);
		CreateShape(points);
	}
	public Vector3[,] GeneratePoints(float radius, int total){
		Vector3[,] points = new Vector3[total + 1, total + 1];
		for (int i = 0; i <= total; i++){
			float lon = Map(i, 0, total, -(float)PI,(float)PI);
			for (int j = 0; j <= total; j++){
				float lat = Map(j, 0, total, -(float)PI * 0.5f, (float)PI * 0.5f);

				float x = (float)(radius * Sin(lon) * Cos(lat));
				float y = (float)(radius * Sin(lon) * Sin(lat));
				float z = (float)(radius * Cos(lon));

				points[i, j] = new Vector3(x, y, z);
			}
		}
		return points;
	}
	public Vector3[,] GenerateSuperPoints(Vector2 coefficients1, Vector2 coefficients2,float r, int total, Vector4 params1, Vector4 params2){
		Vector3[,] points = new Vector3[total + 1, total + 1];
		for (int i = 0; i <= total; i++){
			float lat = Map(i, 0, total, -(float)PI * 0.5f, (float)PI * 0.5f);
			float r2 = SuperRadius(coefficients2, lat, params2.X, params2.Y, params2.Z, params2.W);
			for (int j = 0; j <= total; j++){
				//float radius = perlinNoise.Perlin(i, j);
				float lon = Map(j, 0, total, -(float)PI,(float)PI);
				
				float r1 = SuperRadius(coefficients1, lon, params1.X, params1.Y, params1.Z, params1.W);

				float x = (float)(r * r1 * Cos(lon) * r2 * Cos(lat));
				float y = (float)(r * r1 * Sin(lon) * r2 * Cos(lat));
				float z = (float)(r * r2 * Sin(lat));

				points[i, j] = new Vector3(x, y, z);
			}
		}
		return points;
	}

	public float SuperRadius(Vector2 coefficients, float angle, float m, float n1, float n2, float n3){
		double t1 = (1/coefficients.X) * Cos(m * angle / 4);
		double t2 = (1/coefficients.Y) * Sin(m * angle / 4);
		double r = Pow(Abs(t1), n2) + Pow(Abs(t2), n3);
		return (float)Pow(r, -1/n1);
	}

	public void CreateShape(Vector3[,] points){

		foreach (Node child in shapes.GetChildren())
		{
			child.QueueFree();
		}

		ImmediateMesh mesh = new ImmediateMesh();
		StandardMaterial3D material = new StandardMaterial3D();
		material.VertexColorUseAsAlbedo = true;
		
		for (int i = 0; i < total; i++){
			mesh.SurfaceBegin(Mesh.PrimitiveType.TriangleStrip);
			for (int j = 0; j < total; j++){
				Color vertexColor = new Color(i / (float)total, j / (float)total, 1.0f);

				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i, j]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i + 1, j]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i, j + 1]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i + 1, j]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i + 1, j + 1]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i, j + 1]);

			}
			mesh.SurfaceEnd();
		}

	
		MeshInstance3D sphereInstance = new MeshInstance3D{
			Mesh = mesh,
			MaterialOverride = material
		};

		shapes.AddChild(sphereInstance);
	}

	public void CreateLinedShape(Vector3[,] points)
	{
		foreach (Node child in shapes.GetChildren()){
			child.QueueFree();
		}

		ImmediateMesh mesh = new ImmediateMesh();
		StandardMaterial3D material = new StandardMaterial3D();
		material.VertexColorUseAsAlbedo = true;
		
		Color vertexColor = Colors.Black;
		for (int i = 0; i < total; i++)
		{
			mesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
			for (int j = 0; j < total; j++)
			{
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i, j]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i + 1, j]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i, j + 1]);
				mesh.SurfaceSetColor(vertexColor);
				mesh.SurfaceAddVertex(points[i + 1, j + 1]);
			}
			mesh.SurfaceEnd();
		}

		MeshInstance3D frameInstance = new MeshInstance3D{
			Mesh = mesh,
			MaterialOverride = material
		};

		shapes.AddChild(frameInstance);
	}

	public static float Map(float value, float inMin, float inMax, float outMin, float outMax){
		return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
	}
    public override void _PhysicsProcess(double delta)
    {
        //shapes.RotateX(Mathf.DegToRad(angleRotation));
		//shapes.RotateY(Mathf.DegToRad(angleRotation));
		shapes.RotateZ(Mathf.DegToRad(angleRotation));
    }

	public void SetP1X(float value){
		parameters1.X = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetP1Y(float value){
		parameters1.Y = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetP1Z(float value){
		parameters1.Z = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetP1W(float value){
		parameters1.W = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}

	public void SetP2X(float value){
		parameters2.X = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetP2Y(float value){
		parameters2.Y = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetP2Z(float value){
		parameters2.Z = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetP2W(float value){
		parameters2.W = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}

	public void SetA1(float value){
		coeff1.X = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetB1(float value){
		coeff1.Y = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetA2(float value){
		coeff2.X = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
	public void SetB2(float value){
		coeff2.Y = value;
		GenerateFigure(radius, total, parameters1, parameters2);
	}
}
