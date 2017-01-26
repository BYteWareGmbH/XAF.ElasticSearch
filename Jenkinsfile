node {
	try {
		// Mark the code checkout 'stage'....
		stage 'Checkout'
		// Checkout code from repository
		checkout scm

		// Get the MSBuild tool.
		def msbHome = tool name: 'MSBuild64 14.0', type: 'hudson.plugins.msbuild.MsBuildInstallation'

		// Mark the code build 'stage'....
		stage 'Build'
		// Run MSBuild
		ansiColor('vga') {
			bat '\"${msbHome}\" /m /clp:ForceNoAlign;PerformanceSummary XAF.ElasticSearch.msbuild'
		}
	} catch (e) {
		currentBuild.result = "FAILED"
		emailext( 
			attachLog: true, 
			body: '${JELLY_SCRIPT,template="static-analysis"}', 
			compressLog: true, 
			mimeType: 'text/html', 
			replyTo: '$DEFAULT_REPLYTO', 
			subject: '$DEFAULT_SUBJECT', 
			recipientProviders: [[$class: 'CulpritsRecipientProvider'], [$class: 'DevelopersRecipientProvider'], [$class: 'RequesterRecipientProvider']])
		throw e
	}
}