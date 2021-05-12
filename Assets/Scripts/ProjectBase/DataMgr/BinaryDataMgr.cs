using System.Text;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataWriter
{
    private BinaryWriter writer;
    public GameDataWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }
    public void Write(float value)
    {
        writer.Write(value);
    }
    public void Write(int value)
    {
        writer.Write(value);
    }
    public void Write(Quaternion value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
        writer.Write(value.w);
    }
    public void Write(Vector3 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }
    public void Write(Vector2 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
    }
    public void Write(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }
}

public class GameDataReader
{
    private BinaryReader reader;
    public GameDataReader(BinaryReader reader)
    {
        this.reader = reader;
    }
    public float ReadFloat()
    {
        return reader.ReadSingle();
    }
    public int ReadInt()
    {
        return reader.ReadInt32();
    }
    public Quaternion ReadQuaternion()
    {
        Quaternion value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        value.w = reader.ReadSingle();
        return value;
    }
    public Vector3 ReadVector3()
    {
        Vector3 value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        return value;
    }
    public Vector2 ReadVector2()
    {
        Vector2 value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        return value;
    }
    public string ReadString()
    {
        int length = reader.ReadInt32();
        byte[] bytes = reader.ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }
}

public interface ISaveAble
{
    void Save(GameDataWriter writer);
    void Load(GameDataReader reader, int version);
}

public class BinaryDataMgr : Singleton<BinaryDataMgr>
{
    public string GetSaveFolder()
    {
        return Application.persistentDataPath;
    }
    public string GetSaveFilePath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename);
    }
}
