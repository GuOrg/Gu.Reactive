namespace Gu.Reactive.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class PropertySymbolExt
    {
        internal static bool IsGetOnly(this IPropertySymbol property)
        {
            if (property.ContainingType == KnownSymbol.Nullable ||
                property.ContainingType == KnownSymbol.Type ||
                property.ContainingType == KnownSymbol.TimeSpan ||
                property.ContainingType == KnownSymbol.DateTime ||
                property.ContainingType == KnownSymbol.DateTimeOffset ||
                property.ContainingType == KnownSymbol.String)
            {
                return true;
            }

            if (!TryGetDeclaration(property, out BasePropertyDeclarationSyntax declaration))
            {
                return false;
            }

            if (!TryGetGetter(declaration, out AccessorDeclarationSyntax getter) ||
                getter.Body != null)
            {
                return false;
            }

            return !TryGetSetter(declaration, out AccessorDeclarationSyntax _);
        }

        private static bool TryGetGetter(BasePropertyDeclarationSyntax declaration, out AccessorDeclarationSyntax getter)
        {
            return declaration.TryGetGetAccessorDeclaration(out getter);
        }

        private static bool TryGetSetter(BasePropertyDeclarationSyntax declaration, out AccessorDeclarationSyntax setter)
        {
            return declaration.TryGetSetAccessorDeclaration(out setter);
        }

        private static bool TryGetDeclaration(IPropertySymbol property, out BasePropertyDeclarationSyntax declaration)
        {
            if (property.DeclaringSyntaxReferences.Length != 1)
            {
                declaration = null;
                return false;
            }

            declaration = (BasePropertyDeclarationSyntax)property.DeclaringSyntaxReferences[0].GetSyntax();
            return declaration != null;
        }
    }
}