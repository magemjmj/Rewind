// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffers
{

using global::System;
using global::FlatBuffers;

public struct FInputs : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FInputs GetRootAsFInputs(ByteBuffer _bb) { return GetRootAsFInputs(_bb, new FInputs()); }
  public static FInputs GetRootAsFInputs(ByteBuffer _bb, FInputs obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public static bool FInputsBufferHasIdentifier(ByteBuffer _bb) { return Table.__has_identifier(_bb, "INPU"); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FInputs __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public uint Frame { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
  public bool Up { get { int o = __p.__offset(6); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public bool Down { get { int o = __p.__offset(8); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public bool Left { get { int o = __p.__offset(10); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public bool Right { get { int o = __p.__offset(12); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public bool Jump { get { int o = __p.__offset(14); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }

  public static Offset<FInputs> CreateFInputs(FlatBufferBuilder builder,
      uint frame = 0,
      bool up = false,
      bool down = false,
      bool left = false,
      bool right = false,
      bool jump = false) {
    builder.StartObject(6);
    FInputs.AddFrame(builder, frame);
    FInputs.AddJump(builder, jump);
    FInputs.AddRight(builder, right);
    FInputs.AddLeft(builder, left);
    FInputs.AddDown(builder, down);
    FInputs.AddUp(builder, up);
    return FInputs.EndFInputs(builder);
  }

  public static void StartFInputs(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddFrame(FlatBufferBuilder builder, uint frame) { builder.AddUint(0, frame, 0); }
  public static void AddUp(FlatBufferBuilder builder, bool up) { builder.AddBool(1, up, false); }
  public static void AddDown(FlatBufferBuilder builder, bool down) { builder.AddBool(2, down, false); }
  public static void AddLeft(FlatBufferBuilder builder, bool left) { builder.AddBool(3, left, false); }
  public static void AddRight(FlatBufferBuilder builder, bool right) { builder.AddBool(4, right, false); }
  public static void AddJump(FlatBufferBuilder builder, bool jump) { builder.AddBool(5, jump, false); }
  public static Offset<FInputs> EndFInputs(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FInputs>(o);
  }
  public static void FinishFInputsBuffer(FlatBufferBuilder builder, Offset<FInputs> offset) { builder.Finish(offset.Value, "INPU"); }
  public static void FinishSizePrefixedFInputsBuffer(FlatBufferBuilder builder, Offset<FInputs> offset) { builder.FinishSizePrefixed(offset.Value, "INPU"); }
};


}