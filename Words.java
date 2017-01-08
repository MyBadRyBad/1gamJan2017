import java.util.Set;
import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

public class Words {

    public class WordsGroup {
        public ArrayList<String> synonyms;
        public ArrayList<String> antonyms;

        public WordsGroup() {
            synonyms = new ArrayList<String>();
            antonyms = new ArrayList<String>();
        }
    }

    private static final String synonymURL = "http://pydictionary-geekpradd.rhcloud.com/api/synonym/";
    private static final String antonymURL = "http://pydictionary-geekpradd.rhcloud.com/api/antonym/";

    private static final String synonymsKey = "synonyms";
    private static final String antonymsKey = "antonyms";

    public static void main(String[] args) throws Exception {

        if (args.length != 1) {
            System.out.println("usage: java Words textfile.txt");
        } else {
            Words words = new Words();
           // words.requestRelatedWords("test");
            words.generateWords(args[0]);
        }
    }

    /* 
        synonyms: ["religious,"spiritual","clerical","holy","sectarian","moral"]
        antonyms: ["irreligious","irreverent","evil","immoral","sinful"]
    */

    public JSONObject generateWords(String filename) throws Exception {
        JSONObject jsonsWords = new JSONObject();
        FileInputStream fstream = new FileInputStream(filename);
        BufferedReader br = new BufferedReader(new InputStreamReader(fstream));
        JSONParser parser = new JSONParser();

        String strLine;

        while ((strLine = br.readLine()) != null) {
            if (strLine.length() > 0 && strLine.charAt(0) != '#') {
                JSONArray jsonSynonymArray = (JSONArray)parser.parse(getSynonym(strLine));
                JSONArray jsonAntonymArray = (JSONArray)parser.parse(getAntonym(strLine));

                // add the current word to the list of synonyms since the word is a synonym of itself
                jsonSynonymArray.add(strLine.toLowerCase());
                addWordsToJSONObject(jsonsWords, jsonSynonymArray, jsonAntonymArray);
         //       System.out.println(jsonSynonymArray.toString());
        //        System.out.println(jsonAntonymArray.toString());
            }
        }

        br.close();

        return null;
    }

    private void addWordsToJSONObject(JSONObject jsonObject, JSONArray synonymsArray, JSONArray antonymsArray) {
        for (int index = 0; index < synonymsArray.size(); index++) {
            String synonym = synonymsArray.get(index);

            if (jsonObject.containsKey(synonym)) {
                // add synonyms array to jsonObject synonyms
            }  else {
                jsonObject.set(synonym, copyJSONArray(synonym, synonymsArray));
            }
        }

        for (int index = 0; index < antonymsArray.size(); index++) {
            String antonym = antonymsArray.get(index);

            if (jsonObject.containsKey(antonym)) {
                // add antonyms array to jsonObject antonyms
            } else {
                jsonObject.set(antonym, copyJSONArray(antonym, antonymsArray));
            } 
        }
    }

    private JSONArray copyJSONArray(String keyWord, JSONArray array) {
        JSONArray newArray = new JSONArray();
        for (int index = 0; index < array.size(); index++) {
            String relatedWord = array.get(index);
            if (!relatedWord.equals(keyWord)) 
                newArray.add(relatedWord);
        }

        return newArray;
    }

    private JSONArray combineJSONArray(String keyWord, JSONArray array1, JSONArray array2) {
        Set<String> set = new Set<String>();
        JSONArray newArray1 = copyJSONArray(keyWord, array1);
        JSONArray newArray2 = copyJSONArray(keyWord, array2);

        for (int index = 0; index < newArray1.size(); index++) {

        }

        for (int index = 0; index < newArray2.size(); index++) {
            
        }
    }


    private String getSynonym(String word) throws Exception {
        return requestFromURL(new URL(synonymURL + word));
    }

    private String getAntonym(String word) throws Exception {
        return requestFromURL(new URL(antonymURL + word));
    }

    private String requestFromURL(URL url) throws Exception {
        HttpURLConnection con = (HttpURLConnection) url.openConnection();

        BufferedReader in = new BufferedReader(
                new InputStreamReader(con.getInputStream()));
        String inputLine;
        StringBuffer response = new StringBuffer();

        while ((inputLine = in.readLine()) != null) {
            response.append(inputLine);
        }
        in.close();

        //print result
        return response.toString();
    }

}

