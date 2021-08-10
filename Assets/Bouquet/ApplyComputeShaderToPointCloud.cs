﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pcx;

[ExecuteInEditMode]
public class ApplyComputeShaderToPointCloud : MonoBehaviour
{
    [SerializeField] PointCloudData _sourceData = null;
    [SerializeField] ComputeShader _computeShader = null;

    ComputeBuffer _pointBuffer;

    void OnDisable()
    {
        if(_pointBuffer != null)
        {
            _pointBuffer.Release();
            _pointBuffer = null;
        }
    }

    void Update()
    {
        if(_sourceData == null) { return; }

        var sourceBuffer = _sourceData.computeBuffer;

        if(_pointBuffer == null || _pointBuffer.count != sourceBuffer.count)
        {
            if(_pointBuffer != null) { _pointBuffer.Release(); }
            _pointBuffer = new ComputeBuffer(sourceBuffer.count, PointCloudData.elementSize);
        }

        var time = Application.isPlaying ? Time.time : 0;

        var kernel = _computeShader.FindKernel("CSMain");
        _computeShader.SetFloat("Time", time);
        _computeShader.SetBuffer(kernel, "SourceBuffer", sourceBuffer);
        _computeShader.SetBuffer(kernel, "OutputBuffer", _pointBuffer);
        _computeShader.Dispatch(kernel, sourceBuffer.count / 128, 1, 1);

        GetComponent<PointCloudRenderer>().sourceBuffer = _pointBuffer;
    }
}
