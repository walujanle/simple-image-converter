using ImageMagick;

namespace simple_image_converter
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitializeSecurityPolicies();
            
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        private static void InitializeSecurityPolicies()
        {
            try
            {
                ResourceLimits.MaxMemoryRequest = 512 * 1024 * 1024;
                ResourceLimits.MaxProfileSize = 10 * 1024 * 1024;
                ResourceLimits.Memory = (ulong)(512 * 1024 * 1024);
                ResourceLimits.Disk = (ulong)(1024 * 1024 * 1024);

                var policyXml = @"
                    <policymap>
                      <policy domain=""delegate"" rights=""none"" pattern=""*"" />
                      <policy domain=""coder"" rights=""none"" pattern=""EPHEMERAL"" />
                      <policy domain=""coder"" rights=""none"" pattern=""HTTPS"" />
                      <policy domain=""coder"" rights=""none"" pattern=""HTTP"" />
                      <policy domain=""coder"" rights=""none"" pattern=""URL"" />
                      <policy domain=""coder"" rights=""none"" pattern=""FTP"" />
                      <policy domain=""coder"" rights=""none"" pattern=""MVG"" />
                      <policy domain=""coder"" rights=""none"" pattern=""MSL"" />
                      <policy domain=""path"" rights=""none"" pattern=""@*"" />
                      <policy domain=""cache"" name=""memory-map"" value=""anonymous"" />
                      <policy domain=""cache"" name=""synchronize"" value=""true"" />
                    </policymap>";

                var tempPolicyFile = Path.Combine(Path.GetTempPath(), "magick_policy.xml");
                File.WriteAllText(tempPolicyFile, policyXml);
                
                Environment.SetEnvironmentVariable("MAGICK_CONFIGURE_PATH", Path.GetTempPath());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Warning: Security policy initialization failed.\n{ex.Message}",
                    "Security Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}