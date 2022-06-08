using System.Collections.Generic;
using tModPorter.Rewriters;

namespace tModPorter;

public static partial class Config
{
	public static List<BaseRewriter> CreateRewriters() => new() {
		new RenameRewriter(),
		new InvokeRewriter(),
		new HookSignatureRewriter(),
	};

	static Config() {
		AddModLoaderRefactors();
		AddTerrariaRefactors();
		AddTextureRenames();
	}
}