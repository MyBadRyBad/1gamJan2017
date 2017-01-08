import java.util.Set;
import java.io.FileWriter;
import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

public class Words {
    private static final String KEY_SYNONYMS = "synonyms";
    private static final String KEY_ANTONYMS = "antonyms";

    private static final String synonymURL = "http://pydictionary-geekpradd.rhcloud.com/api/synonym/";
    private static final String antonymURL = "http://pydictionary-geekpradd.rhcloud.com/api/antonym/";

    public static void main(String[] args) throws Exception {

        if (args.length != 2) {
            System.out.println("usage: java Words inputfilename.txt outputfile.json");
        } else {
            Words words = new Words();
            JSONObject object = words.generateWords(args[0]);

            FileWriter fileWriter = new FileWriter(args[1]);
            fileWriter.write(object.toJSONString());
            fileWriter.flush();
            fileWriter.close();
        }
    }
    // TODO need to check for no matches
    //{"error": "Word has no antonym in API"}

    public JSONObject generateWords(String filename) throws Exception {
        JSONObject jsonWords = new JSONObject();
        FileInputStream fstream = new FileInputStream(filename);
        BufferedReader br = new BufferedReader(new InputStreamReader(fstream));
        JSONParser parser = new JSONParser();

        String strLine;

        while ((strLine = br.readLine()) != null) {
            if (strLine.length() > 0 && strLine.charAt(0) != '#') {
                Object synonyms = parser.parse(getSynonym(strLine));
                Object antonyms = parser.parse(getAntonym(strLine));

                JSONArray synonymsArray = (synonyms instanceof JSONArray) ? (JSONArray)synonyms : new JSONArray();
                JSONArray antonymsArray = (antonyms instanceof JSONArray) ? (JSONArray)antonyms : new JSONArray();

                // add the current word to the list of synonyms since the word is a synonym of itself
                synonymsArray.add(strLine.toLowerCase());

                // add the array of words into the json object
                for (int index = 0; index < synonymsArray.size(); index++) {
                    String word = (String)synonymsArray.get(index);
                    addWordToJSONObject(word, jsonWords, synonymsArray, antonymsArray);
                }

                // Antonyms are synonyms of themselves, so swap antonyms and synonyms array
                for (int index = 0; index < antonymsArray.size(); index++) {
                    String word = (String)antonymsArray.get(index);
                    addWordToJSONObject(word, jsonWords, antonymsArray, synonymsArray);
                }

                System.out.println(jsonWords.toJSONString()); 
            }
        }

        br.close();

        return jsonWords;
    }


    private void addWordToJSONObject(String word, JSONObject jsonObject, JSONArray synonymsArray, JSONArray antonymsArray) {
        if (jsonObject.containsKey(word)) {
            JSONObject wordObject = (JSONObject)jsonObject.get(word);
            JSONArray existingSynonymsArray = (JSONArray)wordObject.get(KEY_SYNONYMS);
            JSONArray existingAntonymsArray = (JSONArray)wordObject.get(KEY_ANTONYMS);

            wordObject.put(KEY_SYNONYMS, combineJSONArray(word, existingSynonymsArray, synonymsArray));
            wordObject.put(KEY_ANTONYMS, combineJSONArray(word, existingAntonymsArray, antonymsArray));

            jsonObject.put(word, wordObject);
        }  else {
            JSONObject wordObject = new JSONObject();
            wordObject.put(KEY_SYNONYMS, copyJSONArray(word, synonymsArray));
            wordObject.put(KEY_ANTONYMS, copyJSONArray(word, antonymsArray));

            jsonObject.put(word, wordObject);
        }
    }

    private JSONArray copyJSONArray(String keyWord, JSONArray array) {
        JSONArray newArray = new JSONArray();
        for (int index = 0; index < array.size(); index++) {
            String relatedWord = (String)array.get(index);
            if (!relatedWord.equals(keyWord)) 
                newArray.add(relatedWord);
        }

        return newArray;
    }

    private JSONArray combineJSONArray(String keyWord, JSONArray array1, JSONArray array2) {
        JSONArray newArray = new JSONArray();

  /*      if (array1 != null) {
            for (int index = 0; index < array1.size(); index++) {
            String relatedWord = (String)array1.get(index);
            if (!relatedWord.equals(keyWord)) 
                newArray.add(relatedWord);
            }
        }

        if (array2 != null) {
            BOOL shouldAddWord = (array1 == null) ? (!relatedWord.equals(keyWord)) : 
            (!relatedWord.equals(keyWord) && !newArray.contains(relatedWord)); 

            for (int index = 0; index < array2.size(); index++) {
            String relatedWord = (String)array2.get(index);
            if (shouldAddWord) 
               newArray.add(relatedWord);
            }
        }  */

        for (int index = 0; index < array1.size(); index++) {
            String relatedWord = (String)array1.get(index);
            if (!relatedWord.equals(keyWord)) 
                newArray.add(relatedWord);
        }

        for (int index = 0; index < array2.size(); index++) {
            String relatedWord = (String)array2.get(index);
            if (!relatedWord.equals(keyWord) && !newArray.contains(relatedWord)) 
               newArray.add(relatedWord);
        }

        return newArray;
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

        System.out.println(response.toString());

        return response.toString();
    }

}

