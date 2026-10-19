using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SimplitVPN;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new AssistantForm());
    }
}

internal sealed class AssistantForm : Form
{
    private const string ServerAddress = "vpn.oilmetal.com.ar";
    private const string ProfileName = "Oilmetal VPN";
    private const string InternalNetwork = "172.16.0.0/24";
    private const string DnsSuffix = "oilmetal.com.ar";

    private static readonly Color Navy = Color.FromArgb(20, 43, 73);
    private static readonly Color Blue = Color.FromArgb(32, 112, 196);
    private static readonly Color Background = Color.FromArgb(245, 247, 250);
    private static readonly Color TextColor = Color.FromArgb(39, 51, 66);
    private readonly Button configureButton;
    private readonly Button connectButton;
    private readonly Label statusTitle;
    private readonly Label statusDetails;
    private string? configuredProfileName;

    public AssistantForm()
    {
        var version = typeof(Program).Assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        Text = $"SimplitVPN v{version}";
        ClientSize = new Size(620, 500);
        MinimumSize = new Size(620, 500);
        MaximumSize = new Size(620, 500);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        BackColor = Background;
        Font = new Font("Segoe UI", 10F);

        var header = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(620, 118),
            BackColor = Navy,
            Padding = new Padding(30, 22, 24, 18)
        };
        Controls.Add(header);

        var brand = new Label
        {
            Text = "OILMETAL S.A.",
            ForeColor = Color.FromArgb(184, 210, 238),
            Font = new Font("Segoe UI Semibold", 9F),
            AutoSize = true,
            Location = new Point(30, 18)
        };
        header.Controls.Add(brand);

        var versionLabel = new Label
        {
            Text = $"v{version}",
            ForeColor = Color.FromArgb(184, 210, 238),
            Font = new Font("Segoe UI", 9F),
            Location = new Point(500, 18),
            Size = new Size(90, 18),
            TextAlign = ContentAlignment.MiddleRight
        };
        header.Controls.Add(versionLabel);

        var heading = new Label
        {
            Text = "Conectate a la VPN",
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 21F),
            AutoSize = true,
            Location = new Point(28, 43)
        };
        header.Controls.Add(heading);

        var content = new Panel
        {
            Location = new Point(0, 118),
            Size = new Size(620, 382),
            Padding = new Padding(30, 22, 30, 20),
            BackColor = Background
        };
        Controls.Add(content);

        var intro = new Label
        {
            Text = "Este asistente configura la VPN de Windows",
            ForeColor = TextColor,
            Font = new Font("Segoe UI", 10.5F),
            Location = new Point(30, 20),
            Size = new Size(550, 42)
        };
        content.Controls.Add(intro);

        var infoCard = new Panel
        {
            Location = new Point(30, 76),
            Size = new Size(550, 95),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        content.Controls.Add(infoCard);

        var infoHeading = new Label
        {
            Text = "Tu conexión",
            Font = new Font("Segoe UI Semibold", 10F),
            ForeColor = TextColor,
            Location = new Point(17, 12),
            AutoSize = true
        };
        infoCard.Controls.Add(infoHeading);

        var info = new Label
        {
            Text = $"Servidor  {ServerAddress}     ·     Protocolo  IKEv2\r\nInternet sigue usando tu conexión local; la VPN conecta con la red interna.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(91, 105, 122),
            Location = new Point(17, 39),
            Size = new Size(515, 42)
        };
        infoCard.Controls.Add(info);

        configureButton = new Button
        {
            Text = "Configurar VPN",
            Location = new Point(30, 188),
            Size = new Size(260, 44),
            FlatStyle = FlatStyle.Flat,
            BackColor = Blue,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 10F),
            Cursor = Cursors.Hand
        };
        configureButton.FlatAppearance.BorderSize = 0;
        configureButton.Click += ConfigureButton_Click;
        content.Controls.Add(configureButton);

        connectButton = new Button
        {
            Text = "Conectar",
            Location = new Point(304, 188),
            Size = new Size(276, 44),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Navy,
            Font = new Font("Segoe UI Semibold", 10F),
            Cursor = Cursors.Hand,
            Enabled = false
        };
        connectButton.FlatAppearance.BorderColor = Color.FromArgb(205, 214, 224);
        connectButton.Click += ConnectButton_Click;
        content.Controls.Add(connectButton);

        var statusCard = new Panel
        {
            Location = new Point(30, 250),
            Size = new Size(550, 83),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        content.Controls.Add(statusCard);

        statusTitle = new Label
        {
            Text = "Listo para configurar",
            Font = new Font("Segoe UI Semibold", 9.5F),
            ForeColor = TextColor,
            Location = new Point(16, 12),
            AutoSize = true
        };
        statusCard.Controls.Add(statusTitle);

        statusDetails = new Label
        {
            Text = "Configura el perfil una vez. Windows te pedirá tus credenciales al conectar.",
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.FromArgb(91, 105, 122),
            Location = new Point(16, 38),
            Size = new Size(515, 32)
        };
        statusCard.Controls.Add(statusDetails);

        var footer = new Label
        {
            Text = $"Red interna: {InternalNetwork}     ·     No se guardan contraseñas",
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(112, 124, 138),
            Location = new Point(30, 348),
            AutoSize = true
        };
        content.Controls.Add(footer);
    }

    private async void ConfigureButton_Click(object? sender, EventArgs e)
    {
        configureButton.Enabled = false;
        connectButton.Enabled = false;
        statusTitle.Text = "Configurando el perfil...";
        statusDetails.Text = "Esto puede tardar unos segundos. No cierres esta ventana.";

        try
        {
            var result = await RunPowerShellAsync(BuildProfileScript());
            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException(ExtractError(result));
            }

            configuredProfileName = ExtractProfileName(result.StandardOutput) ?? ProfileName;
            connectButton.Enabled = true;
            statusTitle.Text = "VPN configurada";
            statusDetails.Text = $"El tráfico de Internet queda fuera del túnel. Windows usará la VPN para {InternalNetwork}.";
        }
        catch (Exception ex)
        {
            statusTitle.Text = "No se pudo configurar la VPN";
            statusDetails.Text = ex.Message;
            MessageBox.Show(this, ex.Message, "No se pudo configurar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            configureButton.Enabled = true;
        }
    }

    private void ConnectButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(configuredProfileName))
        {
            return;
        }

        try
        {
            var rasphone = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "rasphone.exe");
            var startInfo = new ProcessStartInfo(rasphone)
            {
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };
            startInfo.ArgumentList.Add("-d");
            startInfo.ArgumentList.Add(configuredProfileName);
            Process.Start(startInfo);
            statusTitle.Text = "Solicitud de conexión abierta";
            statusDetails.Text = "Completa el inicio de sesión en la ventana de Windows.";
        }
        catch (Exception ex)
        {
            statusTitle.Text = "No se pudo iniciar la conexión";
            statusDetails.Text = ex.Message;
        }
    }

    private static string BuildProfileScript() => $$"""
        $ErrorActionPreference = 'Stop'
        $ProgressPreference = 'SilentlyContinue'
        $server = '{{ServerAddress}}'
        $profileName = '{{ProfileName}}'
        $dnsSuffix = '{{DnsSuffix}}'
        $stage = 'inicialización'
        try {
            $stage = 'carga del módulo VPN'
            Import-Module VpnClient -ErrorAction Stop
            $stage = 'búsqueda del perfil'
            $match = @(Get-VpnConnection | Where-Object {
                $_.Name -eq $profileName -or $_.ServerAddress -eq $server
            } | Select-Object -First 1)
            $stage = 'configuración EAP'
            $eap = New-EapConfiguration
            if ($match.Count -gt 0) {
                $profileName = $match[0].Name
                $stage = 'actualización del perfil VPN'
                Set-VpnConnection -Name $profileName -ServerAddress $server -TunnelType Ikev2 `
                    -AuthenticationMethod Eap -EapConfigXmlStream $eap.EapConfigXmlStream `
                    -SplitTunneling:$true -DnsSuffix $dnsSuffix `
                    -RememberCredential:$false | Out-Null
            } else {
                # Add-VpnConnection takes switches here; Set-VpnConnection takes Boolean values.
                $stage = 'creación del perfil VPN'
                Add-VpnConnection -Name $profileName -ServerAddress $server -TunnelType Ikev2 `
                    -AuthenticationMethod Eap -EapConfigXmlStream $eap.EapConfigXmlStream `
                    -SplitTunneling -DnsSuffix $dnsSuffix | Out-Null
            }
            $stage = 'búsqueda de la ruta VPN'
            $connection = Get-VpnConnection -Name $profileName -ErrorAction Stop
            $route = @($connection.Routes | Where-Object { $_.DestinationPrefix -eq '{{InternalNetwork}}' })
            if ($route.Count -eq 0) {
                $stage = 'creación de la ruta VPN'
                Add-VpnConnectionRoute -ConnectionName $profileName -DestinationPrefix '{{InternalNetwork}}' | Out-Null
            }
            Write-Output ('OILMETAL_PROFILE=' + $profileName)
        } catch {
            [Console]::Error.WriteLine(('Error en ' + $stage + ': ' + $_.Exception.Message))
            exit 1
        }
        """;

    private static string? ExtractProfileName(string output)
    {
        const string marker = "OILMETAL_PROFILE=";
        var line = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault(value => value.StartsWith(marker, StringComparison.Ordinal));
        return line is null ? null : line[marker.Length..].Trim();
    }

    private static string ExtractError(PowerShellResult result)
    {
        var error = string.IsNullOrWhiteSpace(result.StandardError) ? result.StandardOutput : result.StandardError;
        return string.IsNullOrWhiteSpace(error) ? "PowerShell devolvió un error sin detalles." : error.Trim();
    }

    private static async Task<PowerShellResult> RunPowerShellAsync(string script)
    {
        var encodedScript = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        var startInfo = new ProcessStartInfo("powershell.exe")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(encodedScript);

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        return new PowerShellResult(await stdoutTask, await stderrTask, process.ExitCode);
    }

    private sealed record PowerShellResult(string StandardOutput, string StandardError, int ExitCode);
}
