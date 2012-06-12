/*
 * This file is part of LuaInterface.
 * 
 * Copyright (C) 2003-2005 Fabio Mascarenhas de Queiroz.
 * Copyright (C) 2009 Joshua Simmons <simmons.44@gmail.com>
 * Copyright (C) 2012 Megax <http://megax.yeahunter.hu/>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using LuaInterface.Extensions;
 
namespace LuaInterface
{
	/// <summary>
	/// Enumeration of basic lua globals.
	/// </summary>
	public enum LuaEnum : int
	{
		/// <summary>
		/// Option for multiple returns in `lua_pcall' and `lua_call'
		/// </summary>
		MultiRet		= -1,
 
		/// <summary>
		/// Everything is OK.
		/// </summary>
		Ok				= 0,
 
		/// <summary>
		/// Thread status, Ok or Yield
		/// </summary>
		Yield			= 1,
 
		/// <summary>
		/// A Runtime error.
		/// </summary>
		ErrorRun		= 2,
 
		/// <summary>
		/// A syntax error.
		/// </summary>
		ErrorSyntax		= 3,
 
		/// <summary>
		/// A memory allocation error. For such errors, Lua does not call the error handler function. 
		/// </summary>
		ErrorMemory		= 4,
 
		/// <summary>
		/// An error in the error handling function.
		/// </summary>
		ErrorError		= 5,
 
		/// <summary>
		/// An extra error for file load errors when using luaL_loadfile.
		/// </summary>
		ErrorFile		= 6,
	}
	
	public enum References : int
	{
		RefNil 	= -1,
		NoRef 	= -2
	}
 
	public enum LuaTypes : int
	{
		None 			= -1,
		Nil 			= 0,
		Boolean 		= 1,
		LightUserdata 	= 2,
		Number 			= 3,
		String 			= 4,
		Table 			= 5,
		Function 		= 6,
		UserData 		= 7,
		Thread 			= 8
	}
 
	public enum GCOption : int
	{
		/// <summary>
		/// Stops the garbage collector.
		/// </summary>
		Stop 			= 0,
 
		/// <summary>
		/// Restarts the garbage collector.
		/// </summary>
		Restart 		= 1,
 
		/// <summary>
		/// Performs a full garbage-collection cycle. 
		/// </summary>
		Collect 		= 2,
 
		/// <summary>
		/// Returns the current amount of memory (in Kbytes) in use by KopiLua.Lua. 
		/// </summary>
		Count 			= 3,
 
		/// <summary>
		/// Returns the remainder of dividing the current amount of bytes of memory in use by Lua by 1024. 
		/// </summary>
		CountB 			= 4,
 
		/// <summary>
		/// Performs an incremental step of garbage collection. The step "size" is controlled by data (larger values mean more steps) in a non-specified way. ifyou want to control the step size you must experimentally tune the value of data. The function returns 1 ifthe step finished a garbage-collection cycle. 
		/// </summary>
		Step 			= 5,
 
		/// <summary>
		/// Sets data as the new value for the pause (Controls how long the collector waits before starting a new cycle) of the collector (see §2.10). The function returns the previous value of the pause.
		/// </summary>
		SetPause 		= 6,
 
		/// <summary>
		/// Sets data as the new value for the step multiplier of the collector (Controls the relative speed of the collector relative to memory allocation.). The function returns the previous value of the step multiplier. 
		/// </summary>
		SetStepMul 		= 7
	}
 
	public enum PseudoIndex : int
	{
		Registry 		= (-10000),
		Environment 	= (-10001),
		Globals 		= (-10002)
	}

	public static class LuaLib
	{
		private static int tag = 0;

		/// <summary>
		/// Function to get byte array from a object
		/// </summary>
		/// <param name="_Object">object to get byte array</param>
		/// <returns>Byte Array</returns>
		public static byte[] ObjectToByteArray(object obj)
		{
			if(obj.IsNull())
				return null;

			var bf = new BinaryFormatter();
			var ms = new MemoryStream();
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}

		public static LuaTypes ToLuaTypes(this int type)
		{
			return (LuaTypes)type;
		}

		public static LuaEnum ToLuaEnum(this int lenum)
		{
			return (LuaEnum)lenum;
		}

		public static bool ToBoolean(this int number)
		{
			return number == 1;
		}

		#region Core Library
		/// <summary>
		/// Pushes a C function onto the stack. This function receives a pointer to a C function and pushes onto the stack a Lua value of type function that, when called, invokes the corresponding C function. 
		/// </summary>
		/// <param name="state">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="fn">
		/// A <see cref="CallbackFunction"/>
		/// </param>
		public static void lua_pushcfunction(KopiLua.Lua.lua_State state, KopiLua.Lua.lua_CFunction fn)
		{
			KopiLua.Lua.lua_pushcclosure(state, fn, 0);
		}
		#endregion

		#region Auxiliary Library
		/// <summary>
		/// Loads and runs the given file.
		/// </summary>
		/// <param name="state">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="filename">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool luaL_dofile(KopiLua.Lua.lua_State state, string filename)
		{
			return (KopiLua.Lua.luaL_loadfile(state, filename).ToLuaEnum() == LuaEnum.Ok) && (KopiLua.Lua.lua_pcall(state, 0, (int)LuaEnum.MultiRet, 0).ToLuaEnum() == LuaEnum.Ok);
		}

		/// <summary>
		/// Loads and runs the given string.
		/// </summary>
		/// <param name="state">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="chunk">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool luaL_dostring(KopiLua.Lua.lua_State state, string chunk)
		{
			return (KopiLua.Lua.luaL_loadstring(state, chunk).ToLuaEnum() == LuaEnum.Ok) && (KopiLua.Lua.lua_pcall(state, 0, (int)LuaEnum.MultiRet, 0).ToLuaEnum() == LuaEnum.Ok);
		}

		public static LuaEnum luaL_loadbuffer(KopiLua.Lua.lua_State luaState, string buff, string name)
		{
			var result = KopiLua.Lua.luaL_loadbuffer(luaState, buff, (uint)buff.Length, name).ToLuaEnum();
			return result;
		}

		/// <summary>
		/// Pops the value referenced by reference by r in the table at index t onto the stack.
		/// </summary>
		/// <param name="state">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="t">
		/// A stack index
		/// </param>
		/// <param name="r">
		/// A <see cref="System.Int32"/>
		/// </param>
		public static void luaL_getref(KopiLua.Lua.lua_State state, int t, int r)
		{
			KopiLua.Lua.lua_rawgeti(state, t, r);	
		}

		public static bool luaL_checkmetatable(KopiLua.Lua.lua_State luaState,int index)
		{
			bool retVal = false;
			Console.WriteLine("v: " + luaState.tt.ToString());

			if(KopiLua.Lua.lua_getmetatable(luaState,index)!=0) 
			{
				KopiLua.Lua.lua_pushlightuserdata(luaState, tag);
				KopiLua.Lua.lua_rawget(luaState, -2);
				retVal = !KopiLua.Lua.lua_isnil(luaState, -1);
				KopiLua.Lua.lua_settop(luaState, -3);
			}

			return retVal;
		}

		public static int luanet_gettag()
		{
			return tag;
		}

		public static void lua_getref(KopiLua.Lua.lua_State luaState, int reference)
		{
			KopiLua.Lua.lua_rawgeti(luaState, (int)PseudoIndex.Registry, reference);
		}

		public static void lua_unref(KopiLua.Lua.lua_State luaState, int reference) 
		{
			KopiLua.Lua.luaL_unref(luaState, (int)PseudoIndex.Registry, reference);
		}

		public static int luanet_rawnetobj(KopiLua.Lua.lua_State luaState,int obj)
		{
			int udata = (int)KopiLua.Lua.lua_touserdata2(luaState, obj);
			return udata != 0 ? udata : -1;
		}

		public static void lua_pushstdcallcfunction(KopiLua.Lua.lua_State luaState,KopiLua.Lua.lua_CFunction function)
		{
			lua_pushcfunction(luaState, function);
		}

		public static int checkudata_raw(KopiLua.Lua.lua_State luaState, int ud, string tname)
		{
			int p = (int)KopiLua.Lua.lua_touserdata2(luaState, ud);
			//Console.WriteLine(BitConverter.ToInt32(ObjectToByteArray(KopiLua.Lua.lua_touserdata(luaState, ud)), 0));
			if(p != 0) 
			{
				/* value is a userdata? */
				if(KopiLua.Lua.lua_getmetatable(luaState, ud)!=0) 
				{
					/* does it have a metatable? */
					KopiLua.Lua.lua_getfield(luaState, (int)PseudoIndex.Registry, tname);  /* get correct metatable */
					bool isEqual = KopiLua.Lua.lua_rawequal(luaState, -1, -2).ToBoolean();

					// NASTY - we need our own version of the lua_pop macro
					// lua_pop(L, 2);  /* remove both metatables */
					KopiLua.Lua.lua_settop(luaState, -(2) - 1);

					if(isEqual)   /* does it have the correct mt? */
						return p;
				 }
			}
		  
			return 0;
		}

		public static int luanet_checkudata(KopiLua.Lua.lua_State luaState, int ud, string tname)
		{
			int udata = checkudata_raw(luaState, ud, tname);
			return udata != 0 ? udata : -1;
		}

		public static void luanet_newudata(KopiLua.Lua.lua_State luaState, int val)
		{
			KopiLua.Lua.lua_newuserdata(luaState, (uint)val);
		}

		public static int luanet_tonetobject(KopiLua.Lua.lua_State luaState, int index)
		{
			int udata;
			Console.WriteLine("x" + KopiLua.Lua.lua_type(luaState, index).ToString());

			if(KopiLua.Lua.lua_type(luaState, index).ToLuaTypes() == LuaTypes.UserData)
			{
				if(luaL_checkmetatable(luaState, index)) 
				{
					udata = (int)KopiLua.Lua.lua_touserdata2(luaState, index);
					if(udata != 0) 
						return udata; 
				}

				udata = checkudata_raw(luaState, index, "luaNet_class");
				if(udata != 0)
					return udata;

				udata = checkudata_raw(luaState, index, "luaNet_searchbase");
				if(udata != 0)
					return udata;

				udata = checkudata_raw(luaState, index, "luaNet_function");
				if(udata != 0)
					return udata;
			}

			return -1;
		}

		public static int lua_ref(KopiLua.Lua.lua_State luaState, int lockRef)
		{
			return lockRef != 0 ? KopiLua.Lua.luaL_ref(luaState, (int)PseudoIndex.Registry) : 0;
		}
		#endregion
	}
}