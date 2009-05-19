using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using System.Runtime.Serialization;
using Mono.Cecil.Cil;

namespace LinFu.Proxy
{
    public class SerializableProxyBuilder : ProxyBuilder 
    {
        public override void Construct(Type originalBaseType, IEnumerable<Type> baseInterfaces, ModuleDefinition module, Mono.Cecil.TypeDefinition targetType)
        {
            var interfaces = new HashSet<Type>(baseInterfaces);
            interfaces.Add(typeof(ISerializable));

            // Create the proxy type
            base.Construct(originalBaseType, interfaces, module, targetType);

            var serializableInterfaceType = module.ImportType<ISerializable>();

            //var methodAttributes = Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.Virtual;
            //var getObjectDataMethod = targetType.DefineMethod("GetObjectData", methodAttributes, typeof(void), typeof(SerializationInfo), typeof(StreamingContext));

            var methods = targetType.Methods;
            var getObjectDataMethod = (from MethodDefinition m in targetType.Methods
                                       where m.Name.Contains("GetObjectData")
                                       select m).First();

            getObjectDataMethod.ImplAttributes = MethodImplAttributes.Managed | MethodImplAttributes.IL;

            var body = getObjectDataMethod.Body;
            body.Instructions.Clear();
            body.InitLocals = true;

            var IL = getObjectDataMethod.GetILGenerator();

            var exceptionCtor = module.ImportConstructor<NotImplementedException>();
            IL.Emit(OpCodes.Newobj, exceptionCtor);
            IL.Emit(OpCodes.Throw);
        }
    }
}
