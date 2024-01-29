// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/asset.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace LibraryManagement.AssetsGRPCService {
  public static partial class AssetManagementGRPCService
  {
    static readonly string __ServiceName = "asset.AssetManagementGRPCService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookAddRequest> __Marshaller_asset_BookAddRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookAddRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookAddResponse> __Marshaller_asset_BookAddResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookAddResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookUpdateRequest> __Marshaller_asset_BookUpdateRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookUpdateRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookUpdateResponse> __Marshaller_asset_BookUpdateResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookUpdateResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookDeleteRequest> __Marshaller_asset_BookDeleteRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookDeleteRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookDeleteResponse> __Marshaller_asset_BookDeleteResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookDeleteResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookCopyAddRequest> __Marshaller_asset_BookCopyAddRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookCopyAddRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookCopyAddResponse> __Marshaller_asset_BookCopyAddResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookCopyAddResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookCopyUpdateRequest> __Marshaller_asset_BookCopyUpdateRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookCopyUpdateRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookCopyUpdateResponse> __Marshaller_asset_BookCopyUpdateResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookCopyUpdateResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookCopyDeleteRequest> __Marshaller_asset_BookCopyDeleteRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookCopyDeleteRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::LibraryManagement.AssetsGRPCService.BookCopyDeleteResponse> __Marshaller_asset_BookCopyDeleteResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::LibraryManagement.AssetsGRPCService.BookCopyDeleteResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::LibraryManagement.AssetsGRPCService.BookAddRequest, global::LibraryManagement.AssetsGRPCService.BookAddResponse> __Method_AddBookRecord = new grpc::Method<global::LibraryManagement.AssetsGRPCService.BookAddRequest, global::LibraryManagement.AssetsGRPCService.BookAddResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "AddBookRecord",
        __Marshaller_asset_BookAddRequest,
        __Marshaller_asset_BookAddResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::LibraryManagement.AssetsGRPCService.BookUpdateRequest, global::LibraryManagement.AssetsGRPCService.BookUpdateResponse> __Method_UpdateBookInfo = new grpc::Method<global::LibraryManagement.AssetsGRPCService.BookUpdateRequest, global::LibraryManagement.AssetsGRPCService.BookUpdateResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "UpdateBookInfo",
        __Marshaller_asset_BookUpdateRequest,
        __Marshaller_asset_BookUpdateResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::LibraryManagement.AssetsGRPCService.BookDeleteRequest, global::LibraryManagement.AssetsGRPCService.BookDeleteResponse> __Method_DeleteBookRecord = new grpc::Method<global::LibraryManagement.AssetsGRPCService.BookDeleteRequest, global::LibraryManagement.AssetsGRPCService.BookDeleteResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "DeleteBookRecord",
        __Marshaller_asset_BookDeleteRequest,
        __Marshaller_asset_BookDeleteResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::LibraryManagement.AssetsGRPCService.BookCopyAddRequest, global::LibraryManagement.AssetsGRPCService.BookCopyAddResponse> __Method_AddBookCopy = new grpc::Method<global::LibraryManagement.AssetsGRPCService.BookCopyAddRequest, global::LibraryManagement.AssetsGRPCService.BookCopyAddResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "AddBookCopy",
        __Marshaller_asset_BookCopyAddRequest,
        __Marshaller_asset_BookCopyAddResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::LibraryManagement.AssetsGRPCService.BookCopyUpdateRequest, global::LibraryManagement.AssetsGRPCService.BookCopyUpdateResponse> __Method_UpdateBookCopy = new grpc::Method<global::LibraryManagement.AssetsGRPCService.BookCopyUpdateRequest, global::LibraryManagement.AssetsGRPCService.BookCopyUpdateResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "UpdateBookCopy",
        __Marshaller_asset_BookCopyUpdateRequest,
        __Marshaller_asset_BookCopyUpdateResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::LibraryManagement.AssetsGRPCService.BookCopyDeleteRequest, global::LibraryManagement.AssetsGRPCService.BookCopyDeleteResponse> __Method_DeleteBookCopy = new grpc::Method<global::LibraryManagement.AssetsGRPCService.BookCopyDeleteRequest, global::LibraryManagement.AssetsGRPCService.BookCopyDeleteResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "DeleteBookCopy",
        __Marshaller_asset_BookCopyDeleteRequest,
        __Marshaller_asset_BookCopyDeleteResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::LibraryManagement.AssetsGRPCService.AssetReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of AssetManagementGRPCService</summary>
    [grpc::BindServiceMethod(typeof(AssetManagementGRPCService), "BindService")]
    public abstract partial class AssetManagementGRPCServiceBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::LibraryManagement.AssetsGRPCService.BookAddResponse> AddBookRecord(global::LibraryManagement.AssetsGRPCService.BookAddRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::LibraryManagement.AssetsGRPCService.BookUpdateResponse> UpdateBookInfo(global::LibraryManagement.AssetsGRPCService.BookUpdateRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::LibraryManagement.AssetsGRPCService.BookDeleteResponse> DeleteBookRecord(global::LibraryManagement.AssetsGRPCService.BookDeleteRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::LibraryManagement.AssetsGRPCService.BookCopyAddResponse> AddBookCopy(global::LibraryManagement.AssetsGRPCService.BookCopyAddRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::LibraryManagement.AssetsGRPCService.BookCopyUpdateResponse> UpdateBookCopy(global::LibraryManagement.AssetsGRPCService.BookCopyUpdateRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::LibraryManagement.AssetsGRPCService.BookCopyDeleteResponse> DeleteBookCopy(global::LibraryManagement.AssetsGRPCService.BookCopyDeleteRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(AssetManagementGRPCServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_AddBookRecord, serviceImpl.AddBookRecord)
          .AddMethod(__Method_UpdateBookInfo, serviceImpl.UpdateBookInfo)
          .AddMethod(__Method_DeleteBookRecord, serviceImpl.DeleteBookRecord)
          .AddMethod(__Method_AddBookCopy, serviceImpl.AddBookCopy)
          .AddMethod(__Method_UpdateBookCopy, serviceImpl.UpdateBookCopy)
          .AddMethod(__Method_DeleteBookCopy, serviceImpl.DeleteBookCopy).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, AssetManagementGRPCServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_AddBookRecord, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::LibraryManagement.AssetsGRPCService.BookAddRequest, global::LibraryManagement.AssetsGRPCService.BookAddResponse>(serviceImpl.AddBookRecord));
      serviceBinder.AddMethod(__Method_UpdateBookInfo, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::LibraryManagement.AssetsGRPCService.BookUpdateRequest, global::LibraryManagement.AssetsGRPCService.BookUpdateResponse>(serviceImpl.UpdateBookInfo));
      serviceBinder.AddMethod(__Method_DeleteBookRecord, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::LibraryManagement.AssetsGRPCService.BookDeleteRequest, global::LibraryManagement.AssetsGRPCService.BookDeleteResponse>(serviceImpl.DeleteBookRecord));
      serviceBinder.AddMethod(__Method_AddBookCopy, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::LibraryManagement.AssetsGRPCService.BookCopyAddRequest, global::LibraryManagement.AssetsGRPCService.BookCopyAddResponse>(serviceImpl.AddBookCopy));
      serviceBinder.AddMethod(__Method_UpdateBookCopy, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::LibraryManagement.AssetsGRPCService.BookCopyUpdateRequest, global::LibraryManagement.AssetsGRPCService.BookCopyUpdateResponse>(serviceImpl.UpdateBookCopy));
      serviceBinder.AddMethod(__Method_DeleteBookCopy, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::LibraryManagement.AssetsGRPCService.BookCopyDeleteRequest, global::LibraryManagement.AssetsGRPCService.BookCopyDeleteResponse>(serviceImpl.DeleteBookCopy));
    }

  }
}
#endregion