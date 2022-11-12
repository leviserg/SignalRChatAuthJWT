using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SignalRChatAuthJWT.Auth
{
    internal static class AuthOptions
    {
        internal const int Lifetime = 1; // время жизни токена - 1 минута
        internal const string Issuer = "MyIssuer";
        internal const string Audience = "MyServer";
        private const string PublicKeyString = "MIIBCgKCAQEAxkbwpeI/7eDpD\u002B9IBVbTWBQFPNfKK96swMurk2JMRzyH/rKJrRnT8q0Rw5ErRqV2fOiIcuauNqbsP3rvdOxc2\u002ByFKZLIhsjnhHTKsyUzOOvYJ397v83PoJT2BMsB2/gVbHbKdEYf1hSX0lswExi/9SQUgrGMSUF8aEmLAMjaV8D3IxUz9i8McHBtvqmK8loLSxisGe920CJKa9pgAZai\u002BjSykSvg0jSqhEjkJI9xhkqAIvGMlXxPud1PAhDaLIm/9VJ9qyuf2g69kbtM3gmT7/6GqjSLwGRbkcSQ/C1MQpsnGGvK\u002Bd412gEAo8SC\u002BXasbY8jsotKVQUlTDjQ444UsQIDAQAB";
        private const string PrivateKeyString = "MIIEowIBAAKCAQEAxkbwpeI/7eDpD\u002B9IBVbTWBQFPNfKK96swMurk2JMRzyH/rKJrRnT8q0Rw5ErRqV2fOiIcuauNqbsP3rvdOxc2\u002ByFKZLIhsjnhHTKsyUzOOvYJ397v83PoJT2BMsB2/gVbHbKdEYf1hSX0lswExi/9SQUgrGMSUF8aEmLAMjaV8D3IxUz9i8McHBtvqmK8loLSxisGe920CJKa9pgAZai\u002BjSykSvg0jSqhEjkJI9xhkqAIvGMlXxPud1PAhDaLIm/9VJ9qyuf2g69kbtM3gmT7/6GqjSLwGRbkcSQ/C1MQpsnGGvK\u002Bd412gEAo8SC\u002BXasbY8jsotKVQUlTDjQ444UsQIDAQABAoIBAC7Ye\u002Br1dZ1CUk5NfnqkdPKOaF5jrYSH69DxTexYgSUjjA4FKLoZLBZeBaBrIApk9YW1eueK0QZgkdi9tu2tGpNrYlrcLzyJIwoMfgetdliwgDV0zUwX2EJcb3PacuoBxy4FMvgdyU/PNb\u002Bhg84/PrswdxgZ0sdMDZSmK41X5x5sMfSmyzEHm7tvJ4UKwiH9SYXCRWS4yZfNFCRn03qSxKlTabjdaJr3DbCYtVi9c\u002BBaPoN9D/gqhijtWlgQs6Z8xJ/HJhA/LvQC0L9rxBbiGaGeBI6du1v2KAHEcOLWDTsWOELZfCCAJdZ1J0DVwEowp/BTAGAM1Z88hXo3B1YkVHECgYEAy4Gb8cQh/w9O3ZCiUhrB/vvLyAZI\u002Bfm3Ku3gx/jghLfVG9d\u002BBXRqgluJpkbtdbSQOvNAbJSVKe7T54Q0IfErNDt9A55fnBbxA4nfa9A3/NrpUAQDsytiJDUfe8ztNvNI5TRSklZ4Dd7eb5VWvkmgwcx3u6NPF/fVPX0w5siFwR8CgYEA\u002BWwGpy7lfgT13eHwFtZpw2GOcUlyRUa730CroBAD1j02vAUEzBVhQmtvdBIYjK\u002BzR2f6V8yd5nJtRt1fqtIDTBje1lCx1t8PoLsi6fAnr8EqidVwGjlS1Sft\u002B7xDUpibA6RzLcJ6FvEZemf0ugpbEp\u002BWgItYrB0AvbP5Z97sYC8CgYEAyDyLGGOHn9OMIe4CCQ9S\u002BnT\u002BmZ21iGDwnUjZ92kmYPAQvAhYcz2M3x76XaVEWKCFmbtnFG9OJahc0FwAf7UwcYBnDXxzTr4z/utV7ls9\u002B4naL2UDzvNM6ZtrkwCcF4lhnETTSjHShrNX4irq4ujHSGjLFtElLcwpAQuBZlK/aLkCgYAyCPywrxS0mhQkhb2fJzYiTsC/cydsREtN9vA29N\u002BAK9l\u002BEFetBSD0rQ63ryWjm1\u002BQlIhA8rg\u002BUXggfipB301wB5E0Vw3F\u002BAtvh/ryCQ49ELgS3HFXoWY/gnoYROUQfbKjIWtFo7zQO1\u002BMEayjYY5xhvgvI0UPieXrKF6A\u002BtRaWwKBgHaGhEB8cfYu5FXLiR8uIG0F2ANXIqjPO/WuBpURmX4NrGk6wYnXeBs6ipbR0QF3\u002B/MOEaS1iZNyokVbmUk9KmHDzhkEFeiR2MV1AiVdN65YlINYrwyrrz7h9\u002BGBjY5MsSfBOFORyTKeJaGozvWCBHzPYeOgwHlAV44OOGz8aa84";

        public static SecurityKey PublicKey = GetPublicKey();
        public static SecurityKey PrivateKey = GetPrivateKey();

        private static SecurityKey GetPrivateKey()
        {
            var key = RSA.Create();
            key.ImportRSAPrivateKey(source: Convert.FromBase64String(PrivateKeyString), bytesRead: out int _);
            return new RsaSecurityKey(key);
        }

        private static SecurityKey GetPublicKey()
        {
            var key = RSA.Create();
            key.ImportRSAPublicKey(source: Convert.FromBase64String(PublicKeyString), bytesRead: out int _);
            return new RsaSecurityKey(key);
        }
    }
}
