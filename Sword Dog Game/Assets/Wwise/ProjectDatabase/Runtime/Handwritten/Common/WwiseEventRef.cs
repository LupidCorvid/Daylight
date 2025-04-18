/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service
 
License Usage
 
Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2025 Audiokinetic Inc.
*******************************************************************************/
#if UNITY_EDITOR
public class WwiseEventRef: global::System.IDisposable
{
    private global::System.IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal WwiseEventRef(global::System.IntPtr cPtr, bool cMemoryOwn)
    {
        swigCMemOwn = cMemoryOwn;
        swigCPtr = cPtr;
        Medias = new WwiseEventRefMediaArray(cPtr);
    }

    internal static global::System.IntPtr getCPtr(WwiseEventRef obj)
    {
        return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(global::System.IntPtr cPtr)
    {
        Dispose();
        swigCPtr = cPtr;
    }

    ~WwiseEventRef()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        global::System.GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        lock (this)
        {
            if (swigCPtr != global::System.IntPtr.Zero)
            {
                if (swigCMemOwn)
                {
                    swigCMemOwn = false;
                    WwiseProjectDatabase.DeleteEventRef(swigCPtr);
                }

                swigCPtr = global::System.IntPtr.Zero;
            }

            global::System.GC.SuppressFinalize(this);
        }
    }
    
    public WwiseEventRef(global::System.IntPtr cPtr) : this(cPtr, true)
    {
    }
    public WwiseEventRef(string eventName) : this(WwiseProjectDatabase.GetEventRefString(eventName), true)
    {
    }
    
    public string Name => WwiseProjectDatabase.GetEventName(swigCPtr);
    public string Path => WwiseProjectDatabase.GetEventPath(swigCPtr);
    public System.IntPtr Guid =>WwiseProjectDatabase.GetEventGuid(swigCPtr);
    public uint ShortId =>WwiseProjectDatabase.GetEventShortId(swigCPtr);
    public float MaxAttenuation => WwiseProjectDatabase.GetEventMaxAttenuation(swigCPtr);
    public float MinDuration => WwiseProjectDatabase.GetEventMinDuration(swigCPtr);
    public float MaxDuration => WwiseProjectDatabase.GetEventMaxDuration(swigCPtr);
    public WwiseEventRefMediaArray Medias { get; }
    public uint MediasCount => WwiseProjectDatabase.GetEventMediasCount(swigCPtr);
}
#endif