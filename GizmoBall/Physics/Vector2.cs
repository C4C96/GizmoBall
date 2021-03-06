﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
	/// <summary>
	/// 表示二维向量的结构体
	/// </summary>
    public struct Vector2
    {
		public float x;
		public float y;

		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// 获得向量的模
		/// </summary>
		public float Magnitude
		{
			get =>  (float) Math.Sqrt(x * x + y * y);
		}

		/// <summary>
		/// 获得单位向量
		/// </summary>
		public Vector2 Normalized
		{
			get => this / Magnitude;
		}

		/// <summary>
		/// 将该向量单位化
		/// </summary>
		public void Normalize()
		{
			this /= Magnitude;
		}

		public override string ToString()
		{
			return $"({x},{y})";
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector2)
				return this == (Vector2) obj;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region Static Member

		private static readonly Vector2 up = new Vector2(0f, -1f);
		private static readonly Vector2 down = new Vector2(0f, 1f);
		private static readonly Vector2 left = new Vector2(-1f, 0f);
		private static readonly Vector2 right = new Vector2(1f, 0f);
		private static readonly Vector2 zero = new Vector2(0f, 0f);

		/// <summary>
		/// 获得向上的单位向量(0, -1)
		/// </summary>
		public static Vector2 Up
		{
			get => up;
		}

		/// <summary>
		/// 获得向下的单位向量(0, 1)
		/// </summary>
		public static Vector2 Down
		{
			get => down;
		}

		/// <summary>
		/// 获得向左的单位向量(-1, 0)
		/// </summary>
		public static Vector2 Left
		{
			get => left;
		}

		/// <summary>
		/// 获得向右的单位向量(1, 0)
		/// </summary>
		public static Vector2 Right
		{
			get => right;
		}

		/// <summary>
		/// 获得零向量(0，0)
		/// </summary>
		public static Vector2 Zero
		{
			get => zero;
		}

		#region Operator Override

		public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
		}

		public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
		}

		public static Vector2 operator -(Vector2 rhs)
		{
			return new Vector2(-rhs.x, -rhs.y);
		}

		public static Vector2 operator *(Vector2 lhs, float rhs)
		{
			return new Vector2(lhs.x * rhs, lhs.y * rhs);
		}

		public static Vector2 operator *(float lhs, Vector2 rhs)
		{
			return new Vector2(lhs * rhs.x, lhs * rhs.y);
		}

		public static Vector2 operator /(Vector2 lhs, float rhs)
		{
			return new Vector2(lhs.x / rhs, lhs.y / rhs);
		}

		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}

		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return !(lhs == rhs);
		}

		#endregion

		#endregion
	}
}
