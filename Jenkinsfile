#!groovy

/*
GNU GENERAL PUBLIC LICENSE
Version 3, 29 June 2007

Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>
Everyone is permitted to copy and distribute verbatim copies
of this license document, but changing it is not allowed.
*/

try {
    notifyBuild('STARTED')
    //Provide the execution node details
    def nodeToExecute = "WINO"
    stage('Prepare code') {
        node (nodeToExecute) {
            gitCheckThatOut('master', 'https://github.com/devwrs/NetCoreLearning.git')
        }
    }
    stage('Build') {
        echo 'Building Project'
        node (nodeToExecute) {
            bat """
                nuget restore IISIntegration.sln
                "${tool('MSBuild')}/MSBuild.exe" "${env.WORKSPACE}"/NetCoreLearning.sln /p:Configuration=Release /p:Platform="Any CPU" /p:ProductVersion=${currentBuildID}
            """
        }
    }
    stage('Test') {
        echo 'Test execution started'
        node (nodeToExecute) {
            if(isUnix()){
                echo 'Running test on Linux'
                //sh 'gradlew clean test'
            }
            else
            {
                echo 'Running test on Windows'
                //bat 'gradlew clean test'
            }
            echo 'Testing - publish test results'
            publishHTML([allowMissing: false,
                         alwaysLinkToLastBuild: false,
                         keepAll: false,
                         reportDir: 'test-output',
                         reportFiles: 'STMExtentReport.html',
                         reportName: 'Test Report',
                         reportTitles: 'Automation Test Report'])
        }
        notifyBuild('DEPLOY TO OCTOPUS?')
        //Waits for user intervention
        timeout(time: 48, unit: 'HOURS') {
            input 'Push to Octopus?'
        }
    }
    // @todo add checkpoint
    stage('Push to Octopus') {
        echo 'Deploying to Octopus'
        node (nodeToExecute) {
            def octopusServer = "http://54.64.6.123:8080"
            def buildArchieveFormat = "zip"
            def buildArchieveLocation = "target"
            def buildArchieveName = env.JOB_NAME + "." + currentBuildID + "." + buildArchieveFormat
            def targetEnvirontment = "DEV"
            if(isUnix()){
                withCredentials([string(credentialsId: 'OctopusAPIKey', variable: 'APIKey')]) {
                    sh """
                        "${tool('Octo CLI AWS')}/Octo" create-project --name ${env.JOB_NAME} --projectgroup "All Projects" --lifecycle "Standard Lifecycle" --ignoreIfExists --server ${octopusServer} --apiKey ${APIKey}
                        "${tool('Octo CLI AWS')}/Octo" pack --id=${env.JOB_NAME} --outFolder=${buildArchieveLocation} --version=${currentBuildID} --format=${buildArchieveFormat}
                        "${tool('Octo CLI AWS')}/Octo" push --package ${buildArchieveLocation}/${buildArchieveName} --replace-existing --server ${octopusServer} --apiKey ${APIKey}
                        "${tool('Octo CLI AWS')}/Octo" create-release --project ${env.JOB_NAME} --releaseNumber=${currentBuildID} --server ${octopusServer} --apiKey ${APIKey}
                    """
                }
            }
            else
            {
                withCredentials([string(credentialsId: 'OctopusAPIKey', variable: 'APIKey')]) {
                    bat """
                        "${tool('Octo CLI Win')}/Octo" create-project --name ${env.JOB_NAME} --projectgroup "All Projects" --lifecycle "Standard Lifecycle" --ignoreIfExists --server ${octopusServer} --apiKey ${APIKey}
                        "${tool('Octo CLI Win')}/Octo" pack --id=${env.JOB_NAME} --outFolder=${buildArchieveLocation} --version=${currentBuildID} --format=${buildArchieveFormat}
                        "${tool('Octo CLI Win')}/Octo" push --package ${buildArchieveLocation}/${buildArchieveName} --replace-existing --server ${octopusServer} --apiKey ${APIKey}
                        "${tool('Octo CLI Win')}/Octo" create-release --project ${env.JOB_NAME} --releaseNumber=${currentBuildID} --server ${octopusServer} --apiKey ${APIKey}
                    """
                }
            }
        }
    }
} catch (e) {
// If there was an exception thrown, the build failed
    currentBuild.result = "FAILED"
    throw e
} finally {
// Success or failure, always send notifications
    notifyBuild(currentBuild.result)
}

/**
 * Clean a Git project workspace.
 * Uses 'git clean' if there is a repository found.
 * Uses Pipeline 'deleteDir()' function if no .git directory is found.
 */
def gitClean() {
    timeout(time: 60, unit: 'SECONDS') {
        if (fileExists('.git')) {
            echo 'Found Git repository: using Git to clean the tree.'
            // The sequence of reset --hard and clean -fdx first
            // in the root and then using submodule foreach
            // is based on how the Jenkins Git SCM clean before checkout
            // feature works.
            sh 'git reset --hard'
            // Note: -e is necessary to exclude the temp directory
            // .jenkins-XXXXX in the workspace where Pipeline puts the
            // batch file for the 'bat' command.
            //sh 'git clean -fd -e "src/vendor/"'
            //sh 'git submodule foreach --recursive git reset --hard'
            //sh 'git submodule foreach --recursive git clean -ffdx'
        }
        else
        {
            echo 'No Git repository found: using deleteDir() to wipe clean'
            deleteDir()
        }
    }
}

/**
 * Notify Build result
 */
def notifyBuild(String buildStatus = 'STARTED') {
    // build status of null means successful
    buildStatus =  buildStatus ?: 'SUCCESSFUL'

    // Default values
    def colorName = 'RED'
    def colorCode = '#FF0000'
    def subject = "${buildStatus}: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]'"
    def summary = "${subject} (${env.BUILD_URL})"
    def details = """<p>STARTED: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]':</p>
    <p>Check console output at &QUOT;<a href='${env.BUILD_URL}'>${env.JOB_NAME} [${env.BUILD_NUMBER}]</a>&QUOT;</p>"""

    // Override default values based on build status
    if ((buildStatus == 'STARTED')||(buildStatus == 'DEPLOY TO STAGING?')) {
        color = 'YELLOW'
        colorCode = '#FFFF00'
    } else if (buildStatus == 'SUCCESSFUL') {
        color = 'GREEN'
        colorCode = '#00FF00'
    } else {
        color = 'RED'
        colorCode = '#FF0000'
    }

    // Send notifications
    slackSend (color: colorCode, message: summary)

    emailext(
            to: 'codetesting@hotmail.com',
            subject: subject,
            body: details,
            recipientProviders: [[$class: 'DevelopersRecipientProvider']]
    )
}

/**
 * Checkout from the Git repository
 */
def gitCheckThatOut(String branch, String vcsUrl) {
    branch =  branch ?: 'master'
    // cleanup
    cleanWs()
    // checkout
    git branch: "${branch}", url: "${vcsUrl}"
    // get last tag
    if(isUnix()) {
        sh "git describe --abbrev=0 --tags > .git/tagName"
    } else {
        bat "git describe --abbrev=0 --tags > .git/tagName"
    }
    def tagName = readFile('.git/tagName').trim()
    // set DisplayName
    currentBuild.displayName = tagName.replace("/","-") + "." + currentBuild.id
    def tagArray = tagName.split("/")
    // Set currentBuild.id
    currentBuildID = tagArray[tagArray.size()-1] + "." + currentBuild.id
    // Print Build ID and Name
    echo "Build Name : ${currentBuild.displayName} :: Build ID : ${currentBuildID}"
}

def tools() {
    maven 'Maven 3.5.2'
    jdk 'Java 8'
}
